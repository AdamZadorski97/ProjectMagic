using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using System;
using Sirenix.OdinInspector;
using DG.Tweening;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private float currentGravity;


    [SerializeField] private Transform meshTransform;
    [SerializeField] private float verticalInput;
    [SerializeField] private Transform headPivot;
    [SerializeField] private Camera playerCamera;
    private float lastStepTime = 0f;
    private bool canWallJump = false;
    private float timeSinceWallHit = 0f;
    private Vector3 lastWallNormal;
    private float shakePhaseOffset = 0f;
    private float accumulatedTime = 0f;
    private bool isClimbing = false;
    private float rollVelocity; // To smooth the roll effect
    private float currentRoll;  // Current roll angle of the camera
    private bool justLanded = false;
    private float currentRotationVelocity;
    private float verticalRotation = 0.0f;
    private Vector3 velocitySmoothDampRef;
    private bool isRunning = false;
    private bool isSliding = false;  // Track if currently sliding
    private float slideTimer = 0;
    private bool isTouchingWall = false;
    private int playerID;
    private InputDevice inputDevice;
    public InputActions playerActions;
    private AudioSource audioSource;
    [ShowInInspector] private float verticalVelocity = 0;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] Vector3 moveDirection;
    private CharacterController characterController;
    private Transform currentPlatform = null;
    private Vector3 lastPlatformPosition = Vector3.zero;
    [SerializeField] private Transform buletSpawnPoint;
    [SerializeField] private float shootingForce = 10f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        SetPlayerActions();
    }
    private void SetPlayerActions()
    {
        // Adjust player ID for 0-based index if necessary
        playerActions = InputController.Instance.GetPlayerActions(playerID - 1);
        if (playerActions == null)
        {
            Debug.LogError($"Player actions not initialized for player ID: {playerID}");
        }
    }
    void Update()
    {
        moveDirection = new Vector3(playerActions.moveValue.x, 0, playerActions.moveValue.y);
        HandleMovement();
        HandleRotation();
        HandleFalling();
        HandleJump();
        HandleSliding();
        HandleRunning();
        HandleClimbing();
        CheckWallCollision();
    }
    public void HandleRunning()
    {
        if (playerActions.runAction.WasPressed)
        {
            isRunning = !isRunning;
        }
    }

    private void HandleClimbing()
    {
        if (!isClimbing)
            return;

        float inputY = playerActions.moveValue.y;
        Vector3 climbDirection = GetClimbDirection(inputY, false);
        if (inputY < 0) CheckClimbEnd(true);
        characterController.Move(climbDirection * Time.deltaTime);

        if (playerActions.jumpAction.WasPressed)
        {
            isClimbing = false;
        }


    }

    private Vector3 GetClimbDirection(float input, bool invert)
    {
        if (input != 0)
        {
            float directionMultiplier = invert ? -1 : 1;
            return new Vector3(0, playerData.ladderClimbSpeed * input * directionMultiplier, 0);
        }
        return Vector3.zero;
    }

    private void CheckClimbEnd(bool shouldStopClimbing)
    {
        if (shouldStopClimbing && IsGrounded())
        {
            isClimbing = false;
        }

    }
    private void StartSliding()
    {
        float slideSpeed = isRunning ? playerData.slideSpeedRun : playerData.slideSpeedWalk;
        if (moveDirection.z < 0)
        {
            return;
        }
        if (moveDirection.z == 0)
        {
            verticalVelocity = Mathf.Sqrt(2 * playerData.jumpForce * playerData.gravity);
            currentVelocity = (transform.forward * playerData.slideSpeedWalk) + new Vector3(0, verticalVelocity, 0);
        }
        else
        {
            currentVelocity = transform.forward * slideSpeed;
        }
        PlayRandomSound(playerData.slideSounds);
        meshTransform.DOScale(new Vector3(1, 0.25f, 1), 0.2f);
        isSliding = true;
        slideTimer = playerData.slideDuration;

        // Set slide velocity in the current forward direction
    }

    public void EnableLadderClimbing(bool enable)
    {
        isClimbing = enable;
        if (enable)
        {
            currentGravity = 0; // Turn off gravity while climbing
        }
        else
        {
            currentGravity = playerData.gravity; // Turn on gravity when not climbing
        }
    }

    private void HandleSliding()
    {
        if (playerActions.slideAction.WasPressed && !isSliding && currentVelocity.magnitude > 0)
        {
            Debug.Log("start Sliding");
            StartSliding();
        }
        if (!isSliding)
            return;

        slideTimer -= Time.deltaTime;


        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * playerData.slideDeceleration);


        if (slideTimer <= 0)
        {
            isSliding = false;  // End the slide
            currentVelocity = Vector3.zero;  // Ensure the character stops if any residual velocity remains
            meshTransform.DOScale(new Vector3(1, 1f, 1), 0.2f);
        }
        Vector3 displacement = new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z) * Time.deltaTime;
        characterController.Move(displacement);
    }
    public void LateUpdate()
    {
        if (IsGrounded())
        {
            ApplyPlatformCorrection();
        }
        else
        {
            transform.parent = null;
            currentPlatform = null;
        }
    }
    void ApplyPlatformCorrection()
    {
        if (currentPlatform != null)
        {
            // Calculate the platform's movement
            Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;

            // If the platform has moved, manually adjust the player's position
            if (platformMovement != Vector3.zero)
            {
                characterController.Move(platformMovement);
            }

            // Always update lastPlatformPosition for the next frame
            lastPlatformPosition = currentPlatform.position;
        }
    }
    public void SetID(int _id, InputDevice _inputDevice)
    {
        playerID = _id;
        inputDevice = _inputDevice;
        playerActions = InputController.Instance.GetPlayerActions(playerID - 1);  // Adjust for 0-based index
        Debug.Log($"SetID called with ID: {playerID} and device: {inputDevice}");
    }
    private void HandleJump()
    {
        if (isSliding)
        {
            return;
        }

        if (playerActions.jumpAction.WasPressed && IsGrounded())
        {
            verticalVelocity = Mathf.Sqrt(2 * playerData.jumpForce * playerData.gravity); // Normal jump
            PlayRandomSound(playerData.jumpSounds);
        }
        else if (playerActions.jumpAction.WasPressed && canWallJump)
        {
            // Wall jump logic
            PlayRandomSound(playerData.jumpSounds);
            Vector3 jumpDirection = Vector3.Reflect(transform.forward, lastWallNormal).normalized;
            verticalVelocity = Mathf.Sqrt(2 * playerData.jumpForce * playerData.gravity); // Wall jump velocity
            currentVelocity = jumpDirection * playerData.walkSpeedMax; // Modify this to set the desired jump strength
            RotateTowards(jumpDirection); // Rotate player to face the jump direction
            canWallJump = false; // Reset wall jump ability
        }
    }
    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        StartCoroutine(SmoothRotate(targetRotation));
    }

    private IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        // Implementing delay
        yield return new WaitForSeconds(playerData.rotationDelay);

        float time = 0;
        while (time < 1)
        {
            float smoothedTime = ApplyEasing(time, playerData.rotationEaseType);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothedTime);
            time += Time.deltaTime / playerData.rotationFromWallSmoothing;
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    private float ApplyEasing(float time, string easeType)
    {
        // Example easing functions; these could be more sophisticated based on needs
        switch (easeType.ToLower())
        {
            case "linear":
                return time;
            case "easein":
                return time * time;
            case "easeout":
                return time * (2 - time);
            case "easeinout":
                return time < 0.5 ? 2 * time * time : -1 + (4 - 2 * time) * time;
            default:
                return time;  // Default to linear if not specified
        }
    }
    private void HandleMovement()
    {
        if (isSliding || isClimbing)
            return;

        Vector3 input = new Vector3(playerActions.moveValue.x, 0, playerActions.moveValue.y);
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 desiredDirection = (input.z * forward + input.x * right).normalized;
        float speed = isRunning ? playerData.runSpeedMax : playerData.walkSpeedMax;
        Vector3 targetVelocity = desiredDirection * speed;

        Vector3 currentHorizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        Vector3 smoothedHorizontalVelocity = Vector3.SmoothDamp(currentHorizontalVelocity, targetVelocity, ref velocitySmoothDampRef, playerData.movementSmoothing);

        currentVelocity = new Vector3(smoothedHorizontalVelocity.x, currentVelocity.y, smoothedHorizontalVelocity.z);
        characterController.Move(currentVelocity * Time.deltaTime);

        // Decide which footstep sounds to play based on speed
        if (isRunning)
        {
            HandleFootsteps(playerData.runFootstepSounds, playerData.stepRateRun);
        }
        else
        {
            HandleFootsteps(playerData.walkFootstepSounds, playerData.stepRateWalk);
        }

        HandleHeadBump();
    }
    private void HandleFootsteps(List<AudioClip> footstepSounds, float stepRate)
    {
        if (currentVelocity.magnitude > 0.1f && Time.time - lastStepTime > stepRate && IsGrounded())
        {
            lastStepTime = Time.time;
            PlayRandomSound(footstepSounds);
        }
    }
    private void PlayRandomSound(List<AudioClip> clips)
    {
        if (clips.Count == 0)
            return;

        int index = UnityEngine.Random.Range(0, clips.Count);
        audioSource.PlayOneShot(clips[index]);
    }
    private void HandleHeadBump()
    {
        if (currentVelocity.magnitude > 0.1f) // Only shake the head when actually moving
        {
            accumulatedTime += Time.deltaTime;

            if (Mathf.Approximately(shakePhaseOffset, 0f))
            {
                // Reset phase offset based on accumulated moving time, providing continuity
                shakePhaseOffset = accumulatedTime * playerData.shakeSpeed;
            }

            float shakeAngle = Mathf.Sin((accumulatedTime * playerData.shakeSpeed) - shakePhaseOffset) * playerData.shakeMagnitude;
            headPivot.transform.localRotation = Quaternion.Euler(new Vector3(shakeAngle, 0, 0));
        }
        else
        {
            // Smoothly reset head rotation when not moving
            headPivot.transform.localRotation = Quaternion.Slerp(headPivot.transform.localRotation, Quaternion.Euler(Vector3.zero), playerData.rotationSmoothing * Time.deltaTime);

            // Reset the shakePhaseOffset and accumulated time for continuity when movement resumes
            shakePhaseOffset = 0f;
            accumulatedTime = 0f;
        }
    }

    private void HandleRotation()
    {
        float horizontalInput = playerActions.lookValue.x; // Assume lookValue.x is for yaw
        float verticalInput = playerActions.lookValue.y; // Assume lookValue.y is for pitch

        // Calculate target rotations

        float cameraRotateSpeed = isRunning ? playerData.angularSpeedRun : playerData.angularSpeedWalk;
        float targetHorizontalRotation = horizontalInput * cameraRotateSpeed;
        float targetVerticalRotation = verticalInput * cameraRotateSpeed;

        // Smoothly interpolate the horizontal rotation speed
        float horizontalRotationAmount = Mathf.Lerp(currentRotationVelocity, targetHorizontalRotation, playerData.rotationSmoothing);
        currentRotationVelocity = horizontalRotationAmount;

        // Apply horizontal rotation
        transform.Rotate(0, horizontalRotationAmount * Time.deltaTime, 0);

        // Handle vertical rotation with clamping
        verticalRotation -= targetVerticalRotation * Time.deltaTime; // Subtract to invert the direction
        verticalRotation = Mathf.Clamp(verticalRotation, -playerData.maxVerticalAngle, playerData.maxVerticalAngle);

        // Apply vertical rotation directly to the camera
        playerCamera.transform.localEulerAngles = new Vector3(verticalRotation, 0, currentRoll);

        // Roll effect based on horizontal rotation
        HandleCameraRoll(horizontalInput);
    }

    private void HandleCameraRoll(float horizontalInput)
    {
        float targetRoll = horizontalInput * -5.0f; // Tweak this value to get the desired roll effect
        currentRoll = Mathf.SmoothDamp(currentRoll, targetRoll, ref rollVelocity, playerData.rotationSmoothing);
    }
    private void HandleFalling()
    {
        if (isClimbing)
            return;

        bool wasGrounded = IsGrounded();
        if (!wasGrounded)
        {
            verticalVelocity -= playerData.gravity * Time.deltaTime;  // Apply gravity if not grounded
        }
        else
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = 0;  // Stop falling when grounded
                if (!justLanded)
                {
                    justLanded = true;
                    // Trigger the squash effect when landing
                    meshTransform.DOScale(new Vector3(1, 0.5f, 1), 0.2f).OnComplete(() =>
                    {
                        meshTransform.DOScale(new Vector3(1, 1, 1), 0.2f);
                    });

                    // Head shake effect on landing
                    ShakeHeadOnLanding();
                }
            }
        }
        currentVelocity = new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z);

        if (wasGrounded && verticalVelocity == 0 && justLanded)
        {
            justLanded = false;  // Reset the landing flag when the player starts moving or after the animation
        }
    }

    private void ShakeHeadOnLanding()
    {
        // Shake the head pivot a little upon landing to add impact
        float shakeDuration = 0.3f; // Duration of the head shake effect
        float shakeStrength = 1f; // Strength of the shake, adjust based on desired intensity
        int shakeVibrato = 10;      // How much vibrato (jitteriness) in the shake
        float shakeRandomness = 90f; // Randomness of the shake angle

        headPivot.DOShakeRotation(shakeDuration, new Vector3(0f, 0f, shakeStrength), shakeVibrato, shakeRandomness, false);
    }
    private void HandleWallCollision()
    {
        if (isSliding)
        {
            Debug.Log("Hit wall while sliding");
            // Add sliding-specific wall collision handling here
        }
        else if (isRunning)
        {
            Debug.Log("Hit wall while running");
            // Add running-specific wall collision handling here
        }
        else
        {
            Debug.Log("Hit wall");
            // General wall collision handling here
        }
    }
    private void CheckWallCollision()
    {
        CollisionFlags flags = characterController.collisionFlags;
        if ((flags & CollisionFlags.Sides) != 0)
        {
            if (!isTouchingWall)
            {
                Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
                Vector3 collisionNormal = GetCollisionNormal(horizontalVelocity);

                // Calculate the velocity component towards the wall using dot product
                float velocityTowardsWall = Vector3.Dot(horizontalVelocity, -collisionNormal);

                if (velocityTowardsWall > playerData.collisionVelocityThreshold)
                {
                    isTouchingWall = true;
                    lastWallNormal = collisionNormal;
                    canWallJump = true;
                    timeSinceWallHit = 0f; // Reset the timer
                    HandleWallCollision();
                }
            }
        }
        else
        {
            if (isTouchingWall)
            {
                isTouchingWall = false;
            }
        }

        // Update the wall jump timer
        if (canWallJump)
        {
            timeSinceWallHit += Time.deltaTime;
            if (timeSinceWallHit > 0.5f) // Time window to wall jump
            {
                canWallJump = false;
            }
        }
    }
    public void TriggerHeadShake(float recoilAmount)
    {
        float shakeIntensity = recoilAmount * 0.1f; // Scale the intensity to a suitable value
        float shakeDuration = 0.1f; // Short, sharp shake
        StartCoroutine(ShakeHead(shakeDuration, shakeIntensity));
    }

    private IEnumerator ShakeHead(float duration, float amount)
    {
        Vector3 originalPosition = headPivot.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = UnityEngine.Random.Range(-1f, 1f) * amount;
            float y = UnityEngine.Random.Range(-1f, 1f) * amount;
            headPivot.localPosition = originalPosition + new Vector3(x, y, 0);
            yield return null;
        }

        headPivot.localPosition = originalPosition;
    }
    private Vector3 GetCollisionNormal(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, 1f))
        {
            return hit.normal;
        }
        else
        {
            return Vector3.zero; // No collision or normal could not be determined
        }
    }




    private bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hit, 1f, playerData.groundMask))
        {
            if (currentPlatform != hit.transform)
            {
                currentPlatform = hit.transform;
                lastPlatformPosition = currentPlatform.position;  // Set initial position for new platform
            }
            return true;
        }
        else
        {
            currentPlatform = null;
            return false;
        }
    }
}
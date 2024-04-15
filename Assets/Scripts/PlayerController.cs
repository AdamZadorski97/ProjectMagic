using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float deceleration = 3f;
    [SerializeField] private float slideDeceleration = 5f;  // Deceleration rate of the slide
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float slideSpeed = 10f;  // Speed of the slide
    [SerializeField] private float slideDuration = 0.5f;  // How long the slide lasts
    [SerializeField] private Transform meshTransform;

    private bool isSliding = false;  // Track if currently sliding
    private float slideTimer = 0;
    private int playerID;
    private InputDevice inputDevice;
    private InputActions playerActions;
    private float verticalVelocity = 0;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    public Vector3 inputDirection;
    private CharacterController characterController;
    private Transform currentPlatform = null;
    private Vector3 lastPlatformPosition = Vector3.zero;
    [SerializeField] private Transform buletSpawnPoint;
    [SerializeField] private float shootingForce = 10f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
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
        HandleMovement();
        HandleFalling();
        HandleJump();
        HandleShooting();
        HandleRotation();

        if (playerActions.slideAction.WasPressed && !isSliding && currentVelocity.magnitude > 0)
        {
            Debug.Log("start Sliding");
            StartSliding();
        }

        if (isSliding)
        {
            UpdateSliding();
        }
    }
    private void StartSliding()
    {
        meshTransform.localScale = new Vector3(1, 0.25f, 1);
        isSliding = true;
        slideTimer = slideDuration;
        currentVelocity = transform.forward * 8;  // Set slide velocity in the current forward direction
    }



    private void UpdateSliding()
    {

        if (!isSliding)
            return;

        slideTimer -= Time.deltaTime;


        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * slideDeceleration);


        if (slideTimer <= 0)
        {
            isSliding = false;  // End the slide
            currentVelocity = Vector3.zero;  // Ensure the character stops if any residual velocity remains
            meshTransform.localScale = new Vector3(1, 1f, 1);  // Reset any transformations made during the slide
        }
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
        if (playerActions.jumpAction.WasPressed && IsGrounded())
        {
            Debug.Log("Jump");
            verticalVelocity = Mathf.Sqrt(2 * jumpForce * gravity); // Initial jump velocity
        }
    }


    private void HandleMovement()
    {
        if (isSliding)
            return;

        inputDirection = new Vector3(playerActions.moveValue.x, 0, playerActions.moveValue.y);
        bool hasInput = inputDirection.sqrMagnitude > 0.01f;

        if (hasInput)
        {
            inputDirection.Normalize();
            float targetSpeed = inputDirection.magnitude * moveSpeed;
            Vector3 targetVelocity = inputDirection.normalized * targetSpeed;
            float smoothTime = (1 / acceleration);
            currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref currentVelocity, smoothTime);
        }
        else
        {
            // When no input is detected, smoothly decelerate to zero
            if (currentVelocity.magnitude > 0.01f)
            {
                float smoothTime = (1 / deceleration);
                currentVelocity = Vector3.SmoothDamp(currentVelocity, Vector3.zero, ref currentVelocity, smoothTime);
            }
            else
            {
                currentVelocity = Vector3.zero; // Ensuring velocity is set to zero when it's close enough
            }
        }

        // Apply gravity and movement
        if (!IsGrounded())
            verticalVelocity -= gravity * Time.deltaTime;
        else
            verticalVelocity = 0;

        Vector3 displacement = new Vector3(currentVelocity.x, verticalVelocity, currentVelocity.z) * Time.deltaTime;
        characterController.Move(displacement);
    }

    private void HandleRotation()
    {
        Vector3 moveDirection = new Vector3(playerActions.moveValue.x, 0, playerActions.moveValue.y);
        Vector3 lookDirection = new Vector3(playerActions.lookValue.x, 0, playerActions.lookValue.y);

        // Choose which direction to face based on the presence of input
        Vector3 direction = (playerActions.lookValue.sqrMagnitude > 0.01) ? lookDirection : ((playerActions.moveValue.sqrMagnitude > 0.01) ? moveDirection : Vector3.zero);

        if (direction != Vector3.zero)
        {
            RotateTowards(direction);
        }
    }
    private void HandleFalling()
    {
        if (!IsGrounded())
        {
            verticalVelocity -= gravity * Time.deltaTime;  // Apply gravity if not grounded
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = 0;  // Stop falling when grounded
        }
    }
    private void HandleShooting()
    {
        if (playerActions.shootAction.WasPressed)
        {
            GameObject bullet = ObjectPool.Instance.GetFromPool();
            bullet.transform.position = buletSpawnPoint.position;  // Set the position to the bullet spawn point
            bullet.GetComponent<BulletController>().SetVelocity(transform.forward);
        }
    }
    private void RotateTowards(Vector3 direction)
    {
        if (isSliding)
            return;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * angularSpeed);
        }
    }
    private bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hit, 1f, groundMask))
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
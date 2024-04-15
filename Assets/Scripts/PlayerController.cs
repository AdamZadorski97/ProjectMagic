using InControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float angularSpeed;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask groundMask;

    private int playerID;
    private InputDevice inputDevice;
    private InputActions playerActions;
    private float verticalVelocity = 0;
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
        Vector3 move = new Vector3(playerActions.moveValue.x, verticalVelocity, playerActions.moveValue.y) * moveSpeed * Time.deltaTime;
        characterController.Move(move);
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
        if (IsGrounded())
            return;
        verticalVelocity -= gravity * Time.deltaTime;
    }
    private void HandleShooting()
    {
        if (playerActions.shootAction.WasPressed)
        {
            GameObject bullet = ObjectPool.Instance.GetFromPool();
            bullet.transform.position = buletSpawnPoint.position;  // Set the position to the bullet spawn point
            bullet.transform.rotation = Quaternion.LookRotation(transform.forward);  // Orient bullet in the direction the player is facing
        }
    }
    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        if (GetComponent<Rigidbody>() != null)
        {
            GetComponent<Rigidbody>().MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime));
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
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
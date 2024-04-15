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


    private Vector2 inputMoveVector;
    private Vector2 inputLookVector;
    private float verticalVelocity = 0;
    private CharacterController characterController;

    [SerializeField] private Transform buletSpawnPoint;
    [SerializeField] private float shootingForce = 10f;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        inputMoveVector = InputController.Instance.player1MoveValue;
        inputLookVector = InputController.Instance.player1LookValue;
        IsGrounded();
        HandleMovement();
        HandleFalling();
        HandleJump();
        HandleShooting();
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0;
        }

        HandleRotation();
    }

    private void HandleJump()
    {
        if (InputController.Instance.Player1Actions.jumpAction.WasPressed && IsGrounded())
        {
            verticalVelocity = Mathf.Sqrt(2 * jumpForce * gravity); // Initial jump velocity
        }
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(inputMoveVector.x, verticalVelocity, inputMoveVector.y) * moveSpeed * Time.deltaTime;
        characterController.Move(move);
    }

    private void HandleRotation()
    {
        Vector3 direction = inputLookVector.sqrMagnitude > 0.1f ? new Vector3(inputLookVector.x, 0, inputLookVector.y) : (inputMoveVector.sqrMagnitude > 0.1f ? new Vector3(inputMoveVector.x, 0, inputMoveVector.y) : Vector3.zero);
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
        if (InputController.Instance.Player1Actions.shootAction.WasPressed)
        {
            GameObject bullet = ObjectPool.Instance.GetFromPool();
            bullet.transform.position = buletSpawnPoint.position;  // Set the position to the bullet spawn point
            bullet.transform.rotation = Quaternion.LookRotation(transform.forward);  // Orient bullet in the direction the player is facing
        }
    }
    private void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), 0.5f, groundMask);
    }
}
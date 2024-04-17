using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirstPersonConfig", menuName = "Configuration/FirstPersonConfig")]
public class PlayerData : ScriptableObject
{
    public float angularSpeed;
    public float jumpForce = 4f;
    public float gravity = 9.81f;
    public float movementSmoothing = 0.1f;
    public float rotationSmoothing = 0.1f;
    public float maxVerticalAngle = 80.0f;
    public float shakeMagnitude = 2.0f;
    public float shakeSpeed = 10.0f;
    public float slideDeceleration = 5f;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public LayerMask groundMask;
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;
    public float collisionVelocityThreshold = 0.5f;
    public float speed = 5.0f;
    public float ladderClimbSpeed = 3.0f;
}
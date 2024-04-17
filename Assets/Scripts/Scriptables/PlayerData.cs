using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FirstPersonConfig", menuName = "Configuration/FirstPersonConfig")]
public class PlayerData : ScriptableObject
{
    public float angularSpeedWalk = 180;
    public float angularSpeedRun = 120;
    public float jumpForce = 4f;
   
    public float gravity = 9.81f;
    public float movementSmoothing = 0.1f;
    public float rotationSmoothing = 0.1f;
  
    public float maxVerticalAngle = 80.0f;


    public LayerMask groundMask;

    [Header("WallJump")]
    public float rotationDelay = 0.2f;  // New
    public string rotationEaseType = "easeInOut";  // New
    public float wallJumpForce = 6f;
    public float rotationFromWallSmoothing = 1.0f;  // Existing
    [Header("HeadBump")]
    public float shakeMagnitude = 2.0f;
    public float shakeSpeed = 10.0f;
    [Header("Move")]
    public float walkSpeedMax = 5f;
    public float runSpeedMax = 15;
    [Header("Slide")]
    public float slideSpeedWalk = 30f;
    public float slideSpeedRun = 60f;
    public float slideDuration = 1f;
    public float slideDeceleration = 5.0f;

    [Header("Ladder")]
    public float ladderClimbSpeed = 3.0f;

    public float collisionVelocityThreshold = 0.5f;
    public float speed = 5.0f;

    [Header("Sounds")]

    public float stepRateWalk = 0.5f; // Time between steps while walking
    public float stepRateRun = 0.3f; // Time between steps while running
    public List<AudioClip> walkFootstepSounds = new List<AudioClip>();
    public List<AudioClip> runFootstepSounds = new List<AudioClip>();
    public List<AudioClip> slideSounds = new List<AudioClip>();
    public List<AudioClip> jumpSounds = new List<AudioClip>();
}
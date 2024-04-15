using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector; // Import Odin Inspector namespace

public class Follower : MonoBehaviour
{
    public enum FollowType
    {
        LerpFollow,
        ConstantFollow
    }

    [Header("Follow Settings")]
    public Transform target;
    public FollowType followType;
    public bool rotateOffset = false;
    public bool rotateToTarget = false; // Option to rotate towards the target

    public Vector3 offset = Vector3.zero;
    public float deadzone = 0.5f;

    [ShowIf("followType", FollowType.LerpFollow)]
    [Header("Lerp Follow Settings")]
    public float lerpFactor = 0.1f;

    [ShowIf("followType", FollowType.ConstantFollow)]
    [Header("Constant Follow Settings")]
    public float followSpeed = 5f;
    public float acceleration = 0.1f;

    private Vector3 currentVelocity = Vector3.zero;

    void Update()
    {
        if (target == null)
            return;

        // Calculate the adjusted position based on whether the offset should be rotated with the target
        Vector3 adjustedOffset = rotateOffset ? target.rotation * offset : offset;
        Vector3 targetPosition = target.position + adjustedOffset;

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > deadzone)
        {
            float effectiveDistance = distance - deadzone; // Distance outside the deadzone
            float normalizedDistance = Mathf.Clamp01(effectiveDistance / deadzone); // Normalize between 0 and 1

            switch (followType)
            {
                case FollowType.LerpFollow:
                    LerpFollow(targetPosition, normalizedDistance);
                    break;
                case FollowType.ConstantFollow:
                    ConstantFollow(targetPosition, normalizedDistance);
                    break;
            }
        }

        if (rotateToTarget)
        {
            RotateTowardsTarget(targetPosition);
        }
    }

    void LerpFollow(Vector3 targetPosition, float normalizedDistance)
    {
        float dynamicLerpFactor = lerpFactor * normalizedDistance;
        transform.position = Vector3.Lerp(transform.position, targetPosition, dynamicLerpFactor);
    }

    void ConstantFollow(Vector3 targetPosition, float normalizedDistance)
    {
        float minSpeed = 0.1f;
        float dynamicSpeed = Mathf.Max(followSpeed * normalizedDistance, minSpeed);
        float smoothTime = 0.5f / acceleration;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime, dynamicSpeed, Time.deltaTime);
    }

    void RotateTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followSpeed);
        }
    }
}
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Speed variables for each axis
    public Vector3 speed;
    public Vector3 targetSpeed;
    public float accelerationTime = 1.0f; // Time in seconds to reach target speed from current speed

    private Vector3 currentSpeed;
    private float timeElapsed; // Time since the speed change began

    void Update()
    {
        // Smoothly interpolate the current speed to the target speed over the specified acceleration time
        if (timeElapsed < accelerationTime)
        {
            timeElapsed += Time.deltaTime;
            currentSpeed = Vector3.Lerp(currentSpeed, targetSpeed, timeElapsed / accelerationTime);
        }
        else
        {
            currentSpeed = targetSpeed; // Ensure it locks to target speed if over time
        }

        // Apply rotation to the object
        transform.Rotate(currentSpeed * Time.deltaTime);
    }

    // Call this method to change the target speed dynamically if needed
    public void ChangeTargetSpeed(Vector3 newTargetSpeed)
    {
        targetSpeed = newTargetSpeed;
        timeElapsed = 0; // Reset time to allow new lerp to happen
    }
}
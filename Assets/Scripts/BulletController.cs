using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;  // Reference to Rigidbody for applying forces or setting velocity
    private ParticleSystem bulletParticles;
    private Collider bulletCollider;
    private void Awake()
    {
        bulletCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        bulletParticles = GetComponentInChildren<ParticleSystem>(); // Assuming the particle system is a child of the bullet
  
    }
    private void OnEnable()
    {
        bulletCollider.enabled = true;
        StopAllCoroutines();
        StartCoroutine(AutoDestroyBullet());
        rb.angularVelocity = Vector3.zero;
 
    }

    public void SetVelocity(Vector3 direction, float bulletSpeed)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = direction * bulletSpeed;
    }

    private IEnumerator AutoDestroyBullet()
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Calculate reflection vector based on the incoming direction and the normal at the point of impact
        Vector3 inDirection = rb.velocity.normalized;
        Vector3 normal = collision.contacts[0].normal; // The normal at the point of impact
        Vector3 reflectDirection = Vector3.Reflect(inDirection, normal).normalized;

        // Check the angle of incidence to adjust if necessary (optional)
        if (Vector3.Angle(reflectDirection, -inDirection) > 170) // Close to 180 degrees means almost parallel
        {
            // Make a minor adjustment to the reflection direction to avoid unnatural behavior
            reflectDirection = Quaternion.Euler(0, 1, 0) * reflectDirection; // Slightly adjust the reflection vector
        }

        // Reduce speed on bounce to simulate energy loss
        float speedAfterCollision = rb.velocity.magnitude * 0.7f; // adjust the factor to tweak energy loss on bounce

        // Set the new velocity based on the reflection direction calculated
        rb.velocity = reflectDirection * speedAfterCollision;

        // Optionally, trigger particle effect on collision
        if (bulletParticles)
        {
            bulletParticles.transform.position = collision.contacts[0].point; // Set particle system to the point of collision
            bulletParticles.Play();
        }

        // Start coroutine to return bullet to pool or destroy after a delay
        StartCoroutine(AutoDestroyBullet());

        // Optionally, disable the collider for a short time to prevent multiple collisions
      
        if (bulletCollider)
        {
            bulletCollider.enabled = false;
            StartCoroutine(EnableColliderAfterDelay());
        }
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(0.2f); // Delay before re-enabling collider, adjust as needed
        bulletCollider.enabled = true;
    }
}
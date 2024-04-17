using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;  // Reference to Rigidbody for applying forces or setting velocity
    private ParticleSystem bulletParticles;
    private Collider bulletCollider;
    public GunController gunController;
    private int hitCount = 0;
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
        hitCount = 0;


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
        hitCount++;  // Increment hit counter

        if (hitCount >= 2)
        {
            // Disable the collider after two hits
            if (bulletCollider)
            {
                bulletCollider.enabled = false;
            }
            // Optionally, stop any further processing since the bullet should no longer interact
            return;
        }
        gunController.PlayRandomSound(gunController.gunData.shootInpactSounds);
        // Calculate reflection vector based on the incoming direction and the normal at the point of impact
        Vector3 inDirection = rb.velocity.normalized;
        Vector3 normal = collision.contacts[0].normal; // The normal at the point of impact
        Vector3 reflectDirection = Vector3.Reflect(inDirection, normal).normalized;
        //if (inDirection.x < 0)
        //{
        //    reflectDirection = new Vector3(-reflectDirection.x, reflectDirection.y, reflectDirection.z);
        //}
        //// Check the angle of incidence to adjust if necessary (optional)
        //if (Vector3.Angle(reflectDirection, -inDirection) > 170) // Close to 180 degrees means almost parallel
        //{
        //    // Make a minor adjustment to the reflection direction to avoid unnatural behavior
        //    reflectDirection = Quaternion.Euler(0, 50, 0) * reflectDirection; // Slightly adjust the reflection vector
        //}

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
      
      
            bulletCollider.enabled = false;
            StartCoroutine(EnableColliderAfterDelay());
        
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForSeconds(1.0f);  // Delay before re-enabling the collider
        if (hitCount < 2 && bulletCollider)
        {
            bulletCollider.enabled = true;
        }
    }
}
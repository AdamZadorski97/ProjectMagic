using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;  // Configurable bullet speed
    private Rigidbody rb;  // Reference to Rigidbody for applying forces or setting velocity

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(AutoDestroyBullet());
 
    }

    public void SetVelocity(Vector3 direction)
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
        // Return this bullet to the pool when it collides with something
        ObjectPool.Instance.ReturnToPool("Bullet", gameObject);
    }
}
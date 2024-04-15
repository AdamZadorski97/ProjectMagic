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
        if (rb == null) rb = GetComponent<Rigidbody>();
        // Apply the initial force based on bulletSpeed and the forward direction
        rb.velocity = transform.forward * bulletSpeed;
    }

    private IEnumerator AutoDestroyBullet()
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnToPool(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Return this bullet to the pool when it collides with something
        ObjectPool.Instance.ReturnToPool(gameObject);
    }
}
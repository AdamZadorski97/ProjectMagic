using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour
{
    public FirstPersonController playerController;
    public GunData gunData;  // Reference to the ScriptableObject storing gun data
    public Transform bulletSpawnPosition;
    private int currentAmmo;
    private float lastShotTime;
    private bool isReloading = false;
    private float nextTimeToFire = 0f; // For handling fire rate

    private void Start()
    {
        currentAmmo = gunData.maxAmmo;  // Initialize ammo
    }

    private void Update()
    {
        if (isReloading)
        {
            return; // Skip update if currently reloading
        }

        if (playerController.playerActions.shootAction.IsPressed && Time.time >= nextTimeToFire)
        {
            if (currentAmmo > 0)
            {
                Shoot();  // Proceed to shoot if there is ammo
            }
            else
            {
                StartCoroutine(Reload());  // Start reloading if ammo is depleted
            }
        }
    }

    private void Shoot()
    {
        nextTimeToFire = Time.time + 1f / gunData.fireRate;  // Schedule the next shot based on the fire rate
        lastShotTime = Time.time;  // Update last shot time
        currentAmmo--;  // Decrement ammo count

        // Instantiate bullet and set its initial properties
        GameObject bullet = ObjectPool.Instance.GetFromPool("Bullet");
        bullet.transform.position = bulletSpawnPosition.position;
        Vector3 shootDirection = bulletSpawnPosition.forward;

        // Apply accuracy spread
        float accuracySpread = Random.Range(-gunData.accuracy, gunData.accuracy);
        shootDirection = Quaternion.Euler(0, accuracySpread, 0) * shootDirection;

        bullet.GetComponent<BulletController>().SetVelocity(shootDirection, gunData.startSpeed);

        //// Apply recoil effect
        //playerController.ApplyRecoil(gunData.recoil);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunData.reloadSpeed);
        currentAmmo = gunData.maxAmmo;
        isReloading = false;
    }
}
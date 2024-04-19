using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunController : MonoBehaviour
{
    public FirstPersonController playerController;
    public GunData gunData;  // Reference to the ScriptableObject storing gun data
    public Transform bulletSpawnPosition;
    private int currentAmmo;
    private float lastShotTime;
    private bool isReloading = false;
    private float nextTimeToFire = 0f; // For handling fire rate
    private AudioSource audioSource;
    private bool gameIsFocused;
    public Transform leftHunHandle;
    public Transform rightHunHandle;
    public void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            gameIsFocused = true;
        }
        else
        {
            gameIsFocused = false;
        }
    }
    private void Start()
    {
        currentAmmo = gunData.maxAmmo;  // Initialize ammo
        // Get the AudioSource component
        gameIsFocused = true;
    }
    private void OnEnable()
    {
        if (rightHunHandle!=null)
        {
            playerController.ik.solver.rightHandEffector.target = rightHunHandle;
            playerController.ik.solver.rightHandEffector.rotationWeight = 1;
            playerController.ik.solver.rightHandEffector.positionWeight = 1;
        }
        else
        {
            playerController.ik.solver.rightHandEffector.target = null;
            playerController.ik.solver.rightHandEffector.rotationWeight = 0;
            playerController.ik.solver.rightHandEffector.positionWeight = 0;
        }
        if(leftHunHandle!=null)
        {
            playerController.ik.solver.leftHandEffector.target = leftHunHandle;
            playerController.ik.solver.leftHandEffector.rotationWeight = 1;
            playerController.ik.solver.leftHandEffector.positionWeight = 1;
        }
        else
        {
            playerController.ik.solver.leftHandEffector.target = null;
            playerController.ik.solver.leftHandEffector.rotationWeight = 0;
            playerController.ik.solver.leftHandEffector.positionWeight = 0;
        }
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if (!gameIsFocused) return;
        if (isReloading)
        {
            return; // Skip update if currently reloading
        }

        if (playerController.playerActions.shootAction.IsPressed && Time.time >= nextTimeToFire)
        {

            TryShoot();  // Proceed to shoot if there is ammo
        }
    }

    private void TryShoot()
    {
   

        nextTimeToFire = Time.time + 1f / gunData.fireRate;  // Schedule the next shot
        lastShotTime = Time.time;
        if (currentAmmo <= 0)
        {
            PlayRandomSound(gunData.emptyMagazineSound);
            StartCoroutine(Reload());
            return;  // Check if there is ammo and play empty magazine sound if not
        }
        currentAmmo--;
        playerController.TriggerHeadShake(gunData);
        PlayRandomSound(gunData.shootSounds);  // Play a random shooting sound

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        Vector3 targetPoint = ray.GetPoint(1000); // Default if no hit

        if (Physics.Raycast(ray, out hit, gunData.maxDistance))
            targetPoint = hit.point;

        Vector3 shootDirection = (targetPoint - bulletSpawnPosition.position).normalized;
        GameObject bullet = ObjectPool.Instance.GetFromPool("Bullet");
        bullet.transform.position = bulletSpawnPosition.position;
        bullet.GetComponent<BulletController>().SetVelocity(shootDirection, gunData.startSpeed);
        bullet.GetComponent<BulletController>().gunController = this;
    }

    private IEnumerator Reload()
    {

        isReloading = true;
        PlayRandomSound(gunData.reloadSounds);
        yield return new WaitForSeconds(gunData.reloadSpeed);
        currentAmmo = gunData.maxAmmo;
        isReloading = false;
    }

    public void PlayRandomSound(List<AudioClip> clips)
    {
        if (clips.Count == 0)
            return; // Return if there are no clips

        int index = Random.Range(0, clips.Count); // Choose a random index
        audioSource.PlayOneShot(clips[index]); // Play the selected audio clip
    }
}
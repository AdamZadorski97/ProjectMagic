using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    public float startSpeed;             // Initial speed of the bullet
    public float bulletDeceleration;     // Deceleration rate of the bullet
    public float maxDistance;            // Maximum shooting distance
    public float recoil;                 // Amount of gun recoil
    public float reloadSpeed;            // Speed of reloading
    public int maxAmmo;                  // Maximum ammunition capacity
    public float fireRate;               // Bullets per second
    public float accuracy;               // Base accuracy of the gun (0 = perfect accuracy, higher values = more spread)
    public float reloadTimeVariability;  // Variability in reload time (to simulate realism in reload speed)
    public List<AudioClip> shootSounds = new List<AudioClip>();
    public List<AudioClip> reloadSounds = new List<AudioClip>();
    public List<AudioClip> emptyMagazineSound = new List<AudioClip>();
}
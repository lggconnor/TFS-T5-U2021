using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class AmmunitionCreator : ScriptableObject
{
    public enum BulletType
    {
        Raycast, // instantaneous bullet, use for fast bullets
        Projectile // instantiates a gameobject, for slow-moving projectiles
    }

    public enum AmmunitionType
    {
        Continuous, // Like a flame thrower
        RateBased,   // Like pistol or machine gun   
        Throwing    // Like a bomb
    }

    public AmmunitionType type;

    public BulletType bulletType;

    public WeaponsList.Weapons weaponName;

    public Sprite sprite;       // UI Image that appears on the canvas for weapon display

    public GameObject spawnPrefab;  // Actual bullets/ammunition to fire

    public GameObject[] spawnEffects;  // Spawn effects at weapon nozzle when player fires weapon

    public Transform spawnPoint;

    public float damageValue;   // Needs to be set after adding in the enemies

    public float fireRate;  // Should be used for Rate Based weapons

    public float range; // Should be used for bullet range

    public int startingBullets; // Number of bullets when the object spawns in the scene

    public int noOfBullets; // Number of Bullets per Shot (only if Max Angle of Deviation > 0)

    public float timeToRange;   // Time to reach the maximum range

    public float scatterRadius; // Should be used for bullet scattering example in a shotgun

    public float maxAngleOfDeviation;   // Error for bullet scattering

    public float fireSpeed; // Applicable for rate based weapons

    public int piercing = 1; // The max number of enemies the bullet will collide with

    public float throwForce;    // Applicable for bombs throw based weapons
}

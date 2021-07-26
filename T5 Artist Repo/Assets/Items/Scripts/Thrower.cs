
using UnityEngine;

[RequireComponent(typeof(Inventory))]
// Attach this script to the player object
public class Thrower : MonoBehaviour
{
    // UI should be made into a Manager Script for loose coupling
    // UI on HUD
    [SerializeField] AmmoDisplay ammoUI;

    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform rightHand;
    [SerializeField] Transform leftHand;

    [SerializeField] AudioClip clip;

    PlayerController player;
    

    // Thrower variables
    public bool isThrowing;
    
    
    [SerializeField] float timeToDestroy = 3f;
    [SerializeField] float hitRange = 7f;

    float nextTimeToFire = 0f;

    GameObject gunEffect;

    // Get information of currently holding items
    Inventory playerInventory;

    void Start()
    {
        playerInventory = GetComponent<Inventory>();
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInventory.SizeOfInventory() <= 0)
        {
            if(ammoUI)
            {
                if (ammoUI.HasWeapons())
                    ammoUI.UpdateAmmo(null);
            }
            
            return;
        }

        if(playerInventory.weaponChanged)
        {
            if(ammoUI)
                ammoUI.UpdateAmmo(playerInventory.currentWeapon);

            Transform spawnPos = playerInventory.currentWeapon.spawnPoint;

            if(spawnPos)
            {
                spawnPoint = spawnPos;
            }

            if (player)
            {
                player.projectileSpawn = spawnPoint;
            }

            playerInventory.weaponChanged = false;
            if (playerInventory.currentWeapon.spawnEffects.Length > 0)
            {
                if (gunEffect)
                    Destroy(gunEffect);
                foreach (GameObject effect in playerInventory.currentWeapon.spawnEffects)
                {
                    if(effect)
                    {
                        gunEffect = Instantiate(effect, spawnPoint.position, spawnPoint.rotation);
                        gunEffect.transform.SetParent(null);
                    }
                    
                }
                
            }
            StopRaycastFire();
        }

        if(isThrowing)
        {
            Debug.Log("Shoot Weapon");
            switch (playerInventory.currentWeapon.type)
            {
                // Rate based fire does the same thing as continuous as of now
                case AmmunitionCreator.AmmunitionType.Continuous:
                case AmmunitionCreator.AmmunitionType.RateBased:

                    UpdateEffectTransform();
                    StartFire();
                    break;

                case AmmunitionCreator.AmmunitionType.Throwing:
                    StartThrowBasedFire();
                    break;
            }
        }
        else
        {
            StopRaycastFire();
        }
            
        
    }

    void UpdateEffectTransform()
    {
        if(isThrowing)
        {
            if(gunEffect)
            {
                gunEffect.transform.position = spawnPoint.position;
                gunEffect.transform.rotation = spawnPoint.rotation;
            }
           
        }
        
    }

    // Call the function OnShoot 
    public void StartFire()
    {
        
        if (Time.time >= nextTimeToFire)
        {
            if (!playerInventory.currentWeapon)
                return;

            if (playerInventory.currentWeapon.noOfBullets > 0)
            {
                
                    if (playerInventory.currentWeapon.spawnEffects.Length > 0)
                    {
                        ThrowRaycastFire();

                    }
                
                // Fire bullets
                nextTimeToFire = Time.time + 1f / playerInventory.currentWeapon.fireRate;
                // Reduce the number of bullets
                playerInventory.currentWeapon.noOfBullets--;
                if (ammoUI)
                    ammoUI.UpdateAmmo(playerInventory.currentWeapon);
                Shoot();
            }
            else
            {
                StopRaycastFire();
            }
        }
        
        
    }

    void Shoot()
    {
        switch (playerInventory.currentWeapon.bulletType)
        {
            case AmmunitionCreator.BulletType.Raycast:
                
                RaycastHit[] hits;
                // add some stuff here to change the orientation of the bullets
                
                hits = Physics.RaycastAll(spawnPoint.position, spawnPoint.forward, playerInventory.currentWeapon.range);
                if (hits.Length <= 0)
                    return;
                for (int i = 0; i < playerInventory.currentWeapon.piercing; i++)
                {
                    // do thing for each enemy
                    if (hits[i].collider.GetComponentInParent<Enemy>() != null)// && hits[i].collider.CompareTag("Enemy"))
                    {
                        hits[i].collider.GetComponentInParent<Enemy>().TakeDamage(playerInventory.currentWeapon.damageValue);
                    }
                    else
                    {
                        i = playerInventory.currentWeapon.piercing;
                    }
                }
                break;

            case AmmunitionCreator.BulletType.Projectile:
                GameObject bullet = Instantiate(playerInventory.currentWeapon.spawnPrefab, spawnPoint.position, spawnPoint.rotation);
                bullet.GetComponent<Rigidbody>().velocity = spawnPoint.forward * playerInventory.currentWeapon.fireSpeed + player._rb.velocity;
                ProjectileDamage dmgScript = bullet.GetComponent<ProjectileDamage>();
                if (dmgScript)
                {
                    dmgScript.damage = playerInventory.currentWeapon.damageValue;
                    dmgScript.piercing = playerInventory.currentWeapon.piercing;
                }
                Destroy(bullet, timeToDestroy);
                break;
        }
    }

    void StartThrowBasedFire()
    {
        if (isThrowing)
            StopRaycastFire();
       
        if (playerInventory.currentWeapon.noOfBullets > 0)
        {
            playerInventory.currentWeapon.noOfBullets--;
            if (ammoUI)
                ammoUI.UpdateAmmo(playerInventory.currentWeapon);
                
            ThrowRanged();
        }
                
        
    }

    void ThrowRanged()
    {
        // Sphere Cast to get nearby objects
       
        GameObject throwObject = Instantiate(playerInventory.currentWeapon.spawnPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = throwObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * playerInventory.currentWeapon.throwForce, ForceMode.VelocityChange);
    }

    void ThrowRaycastFire()
    {
        if (gunEffect != null)
            gunEffect.GetComponent<ParticleSystem>().Play();

        Debug.Log("Should spawn gun effect : " + gunEffect.name);
        isThrowing = true;
    }

    public void StopRaycastFire()
    {
        if(gunEffect != null)
        // gunEffect.transform.SetParent(transform.parent);
            gunEffect.GetComponent<ParticleSystem>().Stop();
        // Debug.Log("Should stop gun effect : " + gunEffect.name);
        isThrowing = false;
    }    
}

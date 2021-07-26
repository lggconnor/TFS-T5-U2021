using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollect : MonoBehaviour
{
    public WeaponsList.Weapons weaponName;

    public int ammoCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(other.GetComponent<Inventory>().CheckInInventory(weaponName) != null)
            {
                other.GetComponent<Inventory>().GetAmmo(weaponName, ammoCount);
                Debug.Log("Player collected " + ammoCount + " ammo for " + weaponName);
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Player doesn't have " + weaponName);
            }
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

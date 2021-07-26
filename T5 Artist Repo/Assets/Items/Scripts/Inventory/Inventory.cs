using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // List housekeeping
    LinkedList<GameObject> itemsList;
    LinkedListNode<GameObject> currentItem;

    [SerializeField] GameObject[] itemsArray;

    // Current Item Properties
    [HideInInspector] public AmmunitionCreator currentWeapon;
    [HideInInspector] public bool weaponChanged;


    // Start is called before the first frame update
    void Start()
    {
        itemsList = new LinkedList<GameObject>();

        if (itemsArray.Length > 0)
        {
            foreach (GameObject item in itemsArray)
            {

                itemsList.AddLast(item);
            }
        }

        currentItem = itemsList.First;

        if(currentItem != null)
            currentWeapon = currentItem.Value.GetComponent<Collectible>().ammunitionCreator;
    }
    // Update is called once per frame
    void Update()
    {
/*
        // Update loop will change when UI for getting the weapon is created
        if (Input.GetKeyDown(KeyCode.Alpha1) && SizeOfInventory() > 0)
        {
            // weaponChanged = true;
           
                // Assuming every collectible/power up has a Collectible script attached
                GetWeapon(itemsList.First); // Get First item

        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && SizeOfInventory() > 1)
        {
            // Assuming every collectible/power up has a Collectible script attached
            GetWeapon(itemsList.First.Next); // Get Second item
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && SizeOfInventory() > 2)
        {

            // Assuming every collectible/power up has a Collectible script attached
            GetWeapon(itemsList.First.Next.Next);    // Get Third item
            

        }*/


    }

    public void WeaponScrollUp()
    {
        if (currentItem != null)
            if (currentItem.Previous != null)
                GetWeapon(currentItem.Previous);
            else
                if (itemsList.Last != null)
                    GetWeapon(itemsList.Last);
    }

    public void WeaponScrollDown()
    {
        if (currentItem != null)
            if (currentItem.Next != null)
                GetWeapon(currentItem.Next);
            else
                if (itemsList.First != null)
                    GetWeapon(itemsList.First);
    }

    void GetWeapon(LinkedListNode<GameObject> getItem)
    {
        if (getItem != null)
        {
            weaponChanged = true;
            currentItem = getItem;  // Get item
            currentWeapon = currentItem.Value.GetComponent<Collectible>().ammunitionCreator;

            if (!currentWeapon.spawnPrefab)
            {
                Debug.Log("No prefab found for " + currentWeapon.name);
            }

            Debug.Log("Current item selected is : " + currentWeapon.name);
        }
    }

    public int SizeOfInventory()
    {
        return itemsList.Count;
    }

    public bool AddToInventory(GameObject item)
    {

        // Check if the item with the "name" exists in the list
        // Assuming every collectible/power up has a Collectible script attached

        if (CheckInInventory(item.GetComponent<Collectible>().ammunitionCreator.weaponName) == null)

        {
            itemsList.AddLast(item);
            return true;
        }
        return false;
    }

    public void GetAmmo(WeaponsList.Weapons weaponName, int ammo)

    {
        if (CheckInInventory(weaponName) != null)
        {
            CheckInInventory(weaponName).Value.GetComponent<Collectible>().ammunitionCreator.noOfBullets += ammo;
        }
        else
        {
            Debug.Log("Player doesn't have " + weaponName);
        }
    }


    public LinkedListNode<GameObject> CheckInInventory(WeaponsList.Weapons name)
    {
        LinkedListNode<GameObject> pointer;
        pointer = itemsList.First;
        while (pointer != null)
        {
            // Assuming every collectible/power up has a Collectible script attached

            if (name == pointer.Value.GetComponent<Collectible>().ammunitionCreator.weaponName)
                return pointer;
            pointer = pointer.Next;
        }

        return null;
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Item"))
        {
            // Don't store duplicate items in the list
            bool shouldAdd = AddToInventory(other.gameObject);
            // Debug.Log("Adding item to the list : " + shouldAdd);
            if(shouldAdd)
            {
                other.gameObject.SetActive(false);
                // Destroy only if new instance of same object has been added to the list
                // Destroy(other.gameObject);
            }

            // First item in list default assignment
            if(SizeOfInventory() == 1)
            {
                currentItem = itemsList.First;
                currentWeapon = currentItem.Value.GetComponent<Collectible>().ammunitionCreator;
           
                Debug.Log("Current item selected is : " + currentWeapon.name);
                weaponChanged = true;
            }
        }
    }
}

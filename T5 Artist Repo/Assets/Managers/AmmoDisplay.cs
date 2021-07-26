using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Need to create a HUD Manager
public class AmmoDisplay : MonoBehaviour
{
    public TMP_Text ammoDisplay;
    public Image weaponSprite;
    bool hasWeapons;

    // Start is called before the first frame update
    void Start()
    {
        ammoDisplay.gameObject.SetActive(false);
        weaponSprite.gameObject.SetActive(false);
    }

    public void UpdateAmmo(AmmunitionCreator weapon)
    {
        if(weapon)
        {
            ammoDisplay.gameObject.SetActive(true);
            weaponSprite.gameObject.SetActive(true);
            ammoDisplay.text = weapon.noOfBullets.ToString();
            weaponSprite.sprite = weapon.sprite;
            hasWeapons = true;
        }
        else
        {
            hasWeapons = false;
            ammoDisplay.gameObject.SetActive(false);
            weaponSprite.gameObject.SetActive(false);
        }
       
    }

    public bool HasWeapons()
    {
        return hasWeapons;
    }
    
}

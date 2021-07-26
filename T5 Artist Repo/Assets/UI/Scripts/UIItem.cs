using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIItem : MonoBehaviour
{
    public AmmunitionCreator ammunition;
    public Image image;
    public Text text;

    CraftingUI craftingUI;
    Button button;

    void Start()
    {
        craftingUI = FindObjectOfType<CraftingUI>();
       
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenWeaponMenu);

        if(!image)
        {
            image = gameObject.transform.GetComponent<Image>();
            image.sprite = ammunition.sprite;
        }
       
        if(!text)
        {
            text = gameObject.transform.parent.GetComponent<Text>();
            text.text = ammunition.weaponName.ToString();
        }
        
    }

    void OpenWeaponMenu()
    {
        if(craftingUI)
        {
            craftingUI.itemName.text = ammunition.weaponName.ToString();
            craftingUI.itemImage.sprite = ammunition.sprite;
            craftingUI.ShowWeaponMenu();
        }
        else
        {
            Debug.LogWarning("No Crafting Menu Found!!");
        }

        // Add Upgrade Buttons using a loop according the number of updates available per weapon
    }
}

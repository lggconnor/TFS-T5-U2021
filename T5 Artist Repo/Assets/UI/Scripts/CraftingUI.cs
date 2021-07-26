using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public GameObject craftingPanel;
    public GameObject itemPanel;
    public Text itemName;
    public Image itemImage;

  
    // Start is called before the first frame update
    void Start()
    {
        HideAllMenus();
    }
    
    public void ShowCraftingMenu()
    {
        craftingPanel.SetActive(true);
        itemPanel.SetActive(false);
    }

    public void ShowWeaponMenu()
    {
        craftingPanel.SetActive(false);
        itemPanel.SetActive(true);
    }
   
    public void HideAllMenus()
    {
        craftingPanel.SetActive(false);
        itemPanel.SetActive(false);
    }

}

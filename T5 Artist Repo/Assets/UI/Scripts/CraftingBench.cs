using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class CraftingBench : MonoBehaviour
{
    
    public CraftingUI craftingUI;
    // [SerializeField] string menuName = "CraftingMenu";
    // Scene craftingMenu;
    PlayerController player;
    bool isLoaded = false;
    bool isInTrigger = false;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // craftingMenu = new Scene();
        if(!craftingUI)
        {
            craftingUI = FindObjectOfType<CraftingUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player should press Action Button to bring up the crafting menu
            isInTrigger = true;
            Debug.Log("Player In");
            player = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player should press Action Button to bring up the crafting menu
            isInTrigger = false;
            Debug.Log("Player Out");
            Debug.Log("Exit Crafting Menu");
            
            UnloadCraftingUI();
            player = null;
        }
    }

    public void OnInteract()
    {
        if(isInTrigger)
        {
            if(!isLoaded)
            {
                Debug.Log("Pressed E");

                LoadCraftingUI();
            }
            else
            {
                Debug.Log("Exit Crafting Menu");
                // SceneManager.UnloadSceneAsync(menuName);
                UnloadCraftingUI();
            }

        }
       // else doesn't work as the function only fires when the Interact button is pressed

    }


    void LoadCraftingUI()
    {
        isLoaded = true;
        if (player)
        {
            player.ToggleInput(false);
        }
        craftingUI.ShowCraftingMenu();
    }

    void UnloadCraftingUI()
    {
        isLoaded = false;
        if (player)
        {
            player.ToggleInput();
        }
        craftingUI.HideAllMenus();
    }
}

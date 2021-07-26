using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class CanvasManager : MonoBehaviour
{
    public Button startGameButton;
    public Button optionsButton;
    public Button extrasButton;
    public Button quitButton;
    public Button restartButton;
    
    //paused
    public Button returnToMenuButton;
    public Button returnToGameButton;



    private bool isPaused;
    public GameObject pauseMenuUI;

    public TMP_Text timeText;
    

   
    // Start is called before the first frame update
    void Start()
    {
        if (startGameButton)
        {
            
            startGameButton.onClick.AddListener(() => SceneManager.LoadScene("Environment Test Scene"));
        }
        if (optionsButton)
        {

            optionsButton.onClick.AddListener(() => SceneManager.LoadScene("OptionsScene"));
        }
        if (extrasButton)
        {

            extrasButton.onClick.AddListener(() => SceneManager.LoadScene("ExtrasScene"));
        }
        if (quitButton)
        {
          
            quitButton.onClick.AddListener(() => SceneManager.LoadScene("GameOverScene"));
        }       
        if (returnToMenuButton)
        {
            returnToMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        }
        if (restartButton)
        {
            restartButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
        }
        if (timeText)
        {
            if (PlayerPrefs.HasKey("LevelTime"))
                timeText.text = "Time Taken : " + PlayerPrefs.GetString("LevelTime") + "s";
            else
                timeText.text = "";
        }
        else
        {
            // If Level complete scene is directly opened without playing the level
            // delete the time stored
            PlayerPrefs.DeleteKey("LevelTime");
        }

    }


    // Update is called once per frame
    private void Update()
    {
        

        if (isPaused)
        {
            ActivateMenu();
        }

        else
        {
            DeactivateMenu();
        }
    }

    void ActivateMenu()
    {
        Time.timeScale = 0;
        //AudioListener.pause = true;
        pauseMenuUI.SetActive(true);
    }

    public void DeactivateMenu()
    {
        Time.timeScale = 1;
        //AudioListener.pause = false;
        pauseMenuUI.SetActive(false);
        isPaused = false;
    }


    public void OnPause()
    {
        isPaused = !isPaused;
    }
}

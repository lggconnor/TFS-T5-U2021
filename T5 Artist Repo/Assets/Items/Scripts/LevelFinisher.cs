using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelFinisher : MonoBehaviour
{
    public TMP_Text timerText;

    private void Update()
    {
        if(timerText)
            timerText.text = Time.time.ToString("F0");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("LevelTime", timerText.text);
            // This should actually be a Level Manager
            // Just for temporary purposes
            SceneManager.LoadScene("LevelComplete");
        }
    }
}

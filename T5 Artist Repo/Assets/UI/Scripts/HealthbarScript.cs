using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    public Slider healthBar;
    public HealthManager healthScript;

    // Start is called before the first frame update
    void Start()
    {
        if (!healthBar)
        {
            healthBar = GetComponent<Slider>();
        }

        if (!healthScript)
        {
            healthScript = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = (float) (healthScript.GetCurrentHealth() / healthScript.GetMaxHP());
    }
}

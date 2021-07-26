using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField]
    private float maxHealth = 10;
    [SerializeField]
    private float currentHealth;

    // public MonoBehaviour controllerScript;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // applies Damage, then returns true if the damage kills the player, and false if the player lives
    public bool Damage(float dmg)
    {
        currentHealth -= dmg;

        Debug.Log(gameObject.name + " has Taken " + dmg + " Damage!");
        // if killed, return True, else return false
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            return true;
        }


        return false;
    }

    // 
    public void Heal(float amt)
    {
        currentHealth += amt;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    public void SetMaxHP(float maxHP)
    {
        if (maxHP > 0)
            maxHealth = maxHP;
    }

    public float GetMaxHP()
    {
        return maxHealth;
    }
}

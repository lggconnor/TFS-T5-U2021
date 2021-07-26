using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    enum TrapType
    {
        SPIKES,
        POISONOUS_GAS
    }
    [SerializeField] TrapType type;

    float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // What should happen if player enters the Trap Collision Field
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            switch (type)
            {
                case TrapType.SPIKES:
                    Debug.Log("Player Entered Spike Trap : " + name);
                    break;
                case TrapType.POISONOUS_GAS:
                    Debug.Log("Player Entered Poisonous Gas Trap : " + name);
                    break;
            }
        }
    }

    // What should happen if player stays in the Trap Collision Field
    private void OnTriggerStay(Collider collider)
    {
        
        if (collider.CompareTag("Player"))
        {

            switch (type)
            {
                case TrapType.SPIKES:
                    // Debug.Log("Player Entered Spike Trap : " + name);
                    break;
                case TrapType.POISONOUS_GAS:
                    if (timer >= 2f) // Reduce player's health after 2 seconds
                                     // Needs to be converted into a variable
                                     // Entire block could be converted into a different function
                    {
                        Debug.Log("Reduce player's health");
                        timer = 0f;
                    }
                    else
                        timer += Time.deltaTime;
                    break;
            }
        }
    }

}

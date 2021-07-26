using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDoodle : MonoBehaviour
{
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        // damage = GetComponentInParent<Enemy>().damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player)
            {
                player.TakeDamage(damage);
            }

            // gameObject.SetActive(false);
        }
    }
}

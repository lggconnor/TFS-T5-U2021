using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    [HideInInspector]
    public float damage = 2f;

    [HideInInspector]
    public int piercing = 1;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(damage);
            }
            piercing--;
            if (piercing <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Debug.Log("There is a collider not set to isTrigger on the Enemy!");
            return;
        }
        if (collision.transform.CompareTag("Player"))
        {
            // Debug.Log("There is a collider not set to isTrigger on the Enemy!");
            return;
        }

        Destroy(gameObject);
    }
}

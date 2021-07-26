using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float destroyTime = 4f;
    [SerializeField] LayerMask mask; // Doesn't do anything yet
    [SerializeField] GameObject acidPool; // If the projectile is an acid projectile
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            Debug.Log("Spawn Acid Pool");
            GameObject acid = Instantiate(acidPool, collision.contacts[0].point, Quaternion.identity);
            acid.transform.localScale *= 0.3f; 
            Destroy(acid, destroyTime);
            Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Damage Player");
            // Damage Player
            Destroy(gameObject);
        }
    }
}

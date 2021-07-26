
using UnityEngine;

// Attach this script to the bomb object
public class Bomb : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float blastRadius = 5f;
    [SerializeField] float explosionForce = 700f;
    float countDown;
    bool hasExploded = false;
    // Start is called before the first frame update
    void Start()
    {
        countDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        // Reduce count down timer
        countDown -= Time.deltaTime;
        if(countDown <= 0 && !hasExploded)
        {
            // Explode only once if the timer has expired
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        Debug.Log("Boom!");
        // Show the effect
        // Instantiate Particles or any effect

        // Get nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach(Collider nearbyObject in colliders)
        {
            // Add force
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();

            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, blastRadius);
            }

            // Damage
            // Apply damage on the shattered pieces 
        }

        // Destroy bomb
        Destroy(gameObject);
    }
}

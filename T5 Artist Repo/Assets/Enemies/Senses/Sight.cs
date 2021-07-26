using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour
{

    // angle of the cone of view from the 'eye' in degrees
    public float sightConeAngle = 100f;
    // distance the that can be seen by the enemy
    public float sightDistance = 30f;

    public float sightUpdateTime = 0.25f;

    public bool isDebug = false;

    // a mask that has only the layer for the target selected. this is used to collect player colliders. 
    // for example, all players would have the layer "Player", and this mask would only have that layer selected
    public LayerMask targetMask;
    // a mask that only has the layer for obstacles selected. this is used to see if the enemy has a line of sight to the player
    public LayerMask obstacleMask;

    // list of the targets that can be seen by this sight node
    public List<Transform> visibleTargets = new List<Transform>();

    Enemy enemyRef;

    // if the player is visible
    // [HideInInspector] public bool isPlayerVisible;
    // Start is called before the first frame update
    void Start()
    {
        // enemyRef = GetComponentInParent<Enemy>();
        StartCoroutine(SightWithDelay(sightUpdateTime));
    }

    /*
    // Update is called once per frame
    void Update()
    {
        Vector3 enemyToPlayerVector;

        foreach (GameObject player in players)
        {
            enemyToPlayerVector = player.transform.position - transform.position;
            float angleBetween = Vector3.Angle(enemyToPlayerVector, transform.forward);

            if (angleBetween < sightConeAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, enemyToPlayerVector, out hit, sightDistance))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        // set the player to visible
                        Debug.Log("A Player is in Sight");

                        if (enemyRef)
                        {
                            enemyRef.canSeePlayer = true;
                            return;
                        }

                    }
                }
            }
        }

        if (enemyRef)
            enemyRef.canSeePlayer = false;
    }
    */

    IEnumerator SightWithDelay(float delay) 
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisiblePlayers();
        }
    }
    

    void FindVisiblePlayers()
    {
        visibleTargets.Clear();

        Collider[] targetsInSightDist = Physics.OverlapSphere(transform.position, sightDistance, targetMask);

        foreach (Collider target in targetsInSightDist)
        {

            Vector3 targetDirVector = target.transform.position - transform.position;
            float distToTarget = Vector3.Magnitude(targetDirVector);
            float angleBetween = Vector3.Angle(targetDirVector, transform.forward);
            if (angleBetween < sightConeAngle / 2)
            {
                if(!Physics.Raycast(transform.position, targetDirVector, distToTarget, obstacleMask))
                {
                    if (isDebug)
                        print(target.transform);
                    visibleTargets.Add(target.transform);
                }
            }
        }
    }
}

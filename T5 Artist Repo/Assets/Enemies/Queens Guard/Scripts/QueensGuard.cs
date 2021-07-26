using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class QueensGuard : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Rigidbody rigidbodyEnemy;

    public Transform target;

    enum EnemyState { Chase, Patrol, Attack };
    [SerializeField] EnemyState state;

    enum PatrolType { DistanceBased, TriggerBased };
    [SerializeField] PatrolType patrolType;

    public bool autoGenPath = false;

    public string pathName;

    public GameObject[] path;

    public int pathIndex = 0;

    public float enemyAttackDistance = 10f;
    //public float enemyFleeDistance = 5f;

    public float damage = 2f;

    public float attackSpeed = 1.5f;
    public float chargedForce = 20.0f;
    

    public float distanceToNextNode;

    public Sight[] sightNodes;

    public bool canSeePlayer = false;

    public bool dead = false;

    HealthManager healthScript;
    // GameObject player;
    private Transform player;

    
    /*
    public GameObject projectilePrefab;
    public Transform spawnPoint;
    

    public float time;

    private float distance;

    private float height;

    public float gravity;

    public float initialXVel = 4f;
    */
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbodyEnemy = GetComponent<Rigidbody>();

        // player = GameObject.FindGameObjectWithTag("Player").transform;
        healthScript = GetComponent<HealthManager>();

        animator.applyRootMotion = false;

       // rigidbodyEnemy.isKinematic = true;
        // rigidbodyEnemy.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (string.IsNullOrEmpty(pathName))
            pathName = "PathNode";

        if (distanceToNextNode <= 0)
            distanceToNextNode = 1.0f;

        if (state == EnemyState.Chase)
            target = GameObject.FindWithTag("Player").transform;
        //enemy run animation
        /*
        else if (state == EnemyState.Patrol)
        {
            if (autoGenPath)
                path = GameObject.FindGameObjectsWithTag(pathName);
        */
        if (path.Length > 0)
            target = path[pathIndex].transform;
        /*   }

           if (autoGenPath)
           {
               GameObject[] tempPath = GameObject.FindGameObjectsWithTag("pathNode");

               if (path.Length == 0 && tempPath.Length > 0)
                   path = tempPath;
               else
               {
                   foreach (GameObject node in path)
                   {
                       if (node == null)
                           path = GameObject.FindGameObjectsWithTag("pathNode");
                       break;
                   }
               }
           }
       */
        if (target)
            agent.SetDestination(target.transform.position);
        // enemy attack animation
        /*
        if (gravity == 0)
            gravity = Physics.gravity.y;

        if (time <= 0)
        {
            time = 4;
        }
        */
        GameObject playerTemp = GameObject.FindGameObjectWithTag("Player");
        if (playerTemp)
            player = playerTemp.transform;
        // Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        CanSeePlayer();

        switch (state)
        {
            case (EnemyState.Attack):
                // Debug.Log("Current State is: " + state);
                transform.LookAt(player.transform.position);
                return;
                break;
            case (EnemyState.Patrol):
                // Debug.Log("Current State is: " + state);
                Patrol(distanceToPlayer);
                break;
            case (EnemyState.Chase):
                // Debug.Log("Current State is: " + state);
                Chase(distanceToPlayer);
                break;

            /*
              case (EnemyState.Flee):
                // Debug.Log("Current State is: " + state);
                Flee(distanceToPlayer);
                break;
            */

            default:
                // Debug.Log("Current State is: " + state);
                Patrol(distanceToPlayer);
                break;
        }
        /*
        if (1 <= animator.GetCurrentAnimatorStateInfo(0).normalizedTime && animator.GetCurrentAnimatorStateInfo(0).IsName("Dying Backwards"))
        {
            SpawnPowerup();
            Destroy(gameObject);
        }
        */
        // If a target exists, tell NavMeshAgent to move towards it
        if (target)
            agent.SetDestination(target.transform.position);
        // Draw a line to see where the 'Enemy' is moving to
        Debug.DrawLine(transform.position, target.transform.position, Color.red);

        // Tell the animator to play animations
        animator.SetBool("IsGrounded", !agent.isOnOffMeshLink);
        animator.SetFloat("Speed", agent.velocity.magnitude);
        // Debug.Log("Enemy is " + state);
    }

    //States 

    void Patrol(float distance)
    {
        if (canSeePlayer)
        {
            // Debug.Log("this should trigger wryy");
            // agent.isStopped = true;

            agent.ResetPath();
            state = EnemyState.Chase;
            target = player;
            // agent.destination = target.transform.position;
            // agent.CalculatePath(target.transform.position, agent.path);
        }
        else if (agent.remainingDistance < distanceToNextNode)
        {
            // Check if there are PathNodes in array
            if (path.Length > 0)
            {
                // Move to the next node
                pathIndex++;
                pathIndex %= path.Length;

                // Update target to next pathNode in array
                target = path[pathIndex].transform;
                // Debug.Log(target);
            }
        }
        else if (path.Length > 0)
            target = path[pathIndex].transform;
    }

    void Chase(float distance)
    {
        /*
        if (distance < enemyFleeDistance)
        {
            target.position = transform.position - (player.position - transform.position).normalized;
            state = EnemyState.Flee;

            return;
        }
        else 
        */
        if (distance < enemyAttackDistance && state != EnemyState.Attack)
        {
            state = EnemyState.Attack;
            StartCoroutine(Attack());
            //target.position = transform.forward * 2;
            //FireProjectile();
            //Debug.Log("Attack Coroutine Started");
            //charged speed towards the player and wait for few secs
            //rigidbodyEnemy.AddForce((target.position - transform.position) * chargedForce);
            
            if (distance <= enemyAttackDistance)
            {
                rigidbodyEnemy.AddForce(transform.forward * chargedForce);

                Debug.Log("ChargedToPlayer");
            }

            
            return;
        }
        else if (!canSeePlayer)
        {
            agent.ResetPath();
            state = EnemyState.Patrol;
        }
    }
    /*
    void Flee(float distance)
    {
        if (distance > enemyFleeDistance)
        {
            state = EnemyState.Chase;
        }
        else
        {
            transform.LookAt(player.position);
            target.position = transform.position - (player.position - transform.position).normalized;
        }
        /*
        else if (!canSeePlayer)
        {
            agent.ResetPath();
            state = EnemyState.Patrol;
        }
        
    }
    */
    IEnumerator Attack()
    {
        // Debug.Log("Enemy Has Started Attacking");
        animator.SetTrigger("SpiderRun");

        //agent.SetDestination(transform.position);
        //agent.isStopped = true;

       

        Debug.Log("Enemy Has Charged towards the player");
        // cooliobeans
        /*
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return new WaitForEndOfFrame();
        */
        yield return new WaitForSeconds(attackSpeed);
        state = EnemyState.Chase;
        //agent.SetDestination(player.transform.position);
        //agent.isStopped = false;
        // Debug.Log("Enemy Has Finished Attacking");
    }

    public void TakeDamage(float dmg)
    {
        if (healthScript.Damage(dmg))
        {
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        dead = true;
        agent.SetDestination(transform.position);
        agent.isStopped = true;
        animator.SetTrigger("Death");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }

    /*
    public void FireProjectile()
    {
        GetHeight();
        GetLength();
        CalculateTime();
        Vector2 velocity = CalculateVelocity(time);
        if (projectilePrefab && spawnPoint)
        {
            GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
            projectile.GetComponent<Rigidbody>().velocity = spawnPoint.up * velocity.y + spawnPoint.forward * velocity.x;
        }
    }

    public void GetHeight()
    {
        if (player && spawnPoint)
        {
            height = player.position.y - spawnPoint.position.y;
        }
    }

    public void GetLength()
    {
        if (player && spawnPoint)
        {
            Vector3 playerXZ = new Vector3(player.position.x, 0, player.position.z);
            Vector3 selfXZ = new Vector3(spawnPoint.position.x, 0, spawnPoint.position.z);
            distance = Vector3.Distance(playerXZ, selfXZ);
        }
    }

    public void CalculateTime()
    {
        if (initialXVel > 0)
            time = distance / initialXVel;
    }

    public Vector2 CalculateVelocity(float time)
    {
        Vector2 velocity = new Vector2();
        if (initialXVel <= 0)
            velocity.x = distance / time;
        else
            velocity.x = initialXVel;
        velocity.y = (height / time) - (gravity * time / 2);

        return velocity;
    }
    */
    public void CanSeePlayer()
    {
        if (sightNodes.Length > 0)
        {
            foreach (Sight node in sightNodes)
            {
                if (node.visibleTargets.Count > 0)
                {
                    /*
                    foreach (Transform target in node.visibleTargets)
                    {
                        if (Vector3.Distance(target.position, node.transform.position) < Vector3.Distance(this.target.transform.position, node.transform.position))
                        {
                            this.target = target.gameObject;
                        }
                    }
                    */
                    canSeePlayer = true;
                    return;
                }
            }
        }
        canSeePlayer = false;
    }
}

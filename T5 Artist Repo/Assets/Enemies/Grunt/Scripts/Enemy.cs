using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    Rigidbody rigidbodyEnemy;

    public GameObject target;

    enum EnemyState { Chase, Patrol, Attack };
    [SerializeField] EnemyState state;

    enum PatrolType { DistanceBased, TriggerBased };
    [SerializeField] PatrolType patrolType;

    public bool autoGenPath = false;

    public string pathName;

    public GameObject[] path;

    public int pathIndex = 0;

    public float enemyAttackDistance = 1f;

    public float damage = 2f;

    public float distanceToNextNode;

    public bool canSeePlayer = false;

    public bool dead = false;

    HealthManager healthScript;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbodyEnemy = GetComponent<Rigidbody>();

        player = GameObject.FindGameObjectWithTag("Player");
        healthScript = GetComponent<HealthManager>();

        animator.applyRootMotion = false;

        rigidbodyEnemy.isKinematic = true;
        rigidbodyEnemy.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

        if (string.IsNullOrEmpty(pathName))
            pathName = "PathNode";

        if (distanceToNextNode <= 0)
            distanceToNextNode = 1.0f;

        if (state == EnemyState.Chase)
            target = GameObject.FindWithTag("Player");
        //enemy run animation

        else if (state == EnemyState.Patrol)
        {
            if (autoGenPath)
                path = GameObject.FindGameObjectsWithTag(pathName);

            if (path.Length > 0)
                target = path[pathIndex];
        }

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
                        path = path = GameObject.FindGameObjectsWithTag("pathNode");
                    break;
                }
            }
        }
        if (target)
            agent.SetDestination(target.transform.position);
        // enemy attack animation
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (state)
        {
            case (EnemyState.Patrol):
                Patrol(distanceToPlayer);
                break;
            case (EnemyState.Chase):
                Chase(distanceToPlayer);
                break;
            case (EnemyState.Attack):
                transform.LookAt(player.transform.position);
                return;
                break;
            default:
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
                target = path[pathIndex];
                // Debug.Log(target);
            }
        }
        else if (path.Length > 0)
            target = path[pathIndex];
    }

    void Chase(float distance)
    {
        if (distance < enemyAttackDistance)
        {
            state = EnemyState.Attack;
            StartCoroutine(Attack());
            return;
        }
        else if (!canSeePlayer)
        {
            agent.ResetPath();
            state = EnemyState.Patrol;
        }
    }

    IEnumerator Attack()
    {
        Debug.Log("Enemy Has Started Attacking");
        animator.SetTrigger("SpiderAttack");
        agent.SetDestination(transform.position);
        // cooliobeans
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            yield return new WaitForEndOfFrame();
        state = EnemyState.Chase;
        agent.SetDestination(player.transform.position);
        Debug.Log("Enemy Has Finished Attacking");
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


    // Usage:
    // -Add a collider
    //- Add Rigidbody that is set to IsKenematic
    void OnTriggerEnter(Collider other)
    {

    }
}

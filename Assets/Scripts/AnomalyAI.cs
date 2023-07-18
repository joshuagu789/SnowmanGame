using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/* 
 * Early script created for Anomaly NPC's movement- better versions of movement for NPCs can be found w/ EnemyMovement and SnowmanMovement
 */

public class AnomalyAI : MonoBehaviour
{

    public NavMeshAgent anomaly;
    //public Transform player;
    //public LayerMask whatIsGround, whatIsPlayer;
    private Animator animator;

    // For interactions with player
    public Player player;

    public Transform rangeFinder;
    bool extinguished;

    // For patrolling movement
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange = 40f;
    public float turnSmoothTime = 0.5f;
    float turnSmoothVelocity;

    // For being idle
    bool isIdle;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    void Awake()
    {
        anomaly = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Idle();
        Patrolling();
        BurnEnemies();
    }

    void Idle()
    {
        // Making anomaly idle for a period of time
        if (isIdle)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                isIdle = false;

        // 50% probability of being idle if anomaly isn't patrolling
        }
        else if (Random.value >= 0.5f && !walkPointSet)
        {
            timer = Random.value * 30f;
            isIdle = true;
            animator.SetBool("isMoving", false);
        }
    }

    void Patrolling()
    {
        if (!walkPointSet && !isIdle)
        {
            SearchWalkPoint();
        }

        // If walkPoint is ready
        else if(walkPointSet)
        {
            anomaly.SetDestination(walkPoint);

            // Checking if destination is reached
            Vector3 distanceToWalkPoint = walkPoint - transform.position;
            if (distanceToWalkPoint.magnitude < 0.25f)
            {
                walkPointSet = false;
            }
        }
    }

    void SearchWalkPoint()
    {
        //Picking random coordinates to travel
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Checking if destination is within map     EXPERIMENTAL: have raycast go up as well if destination is at a higher
        //                                                          elevation (random coordinate assumes y position doesn't change)
        if (Physics.Raycast(walkPoint, -transform.up) || Physics.Raycast(walkPoint, -transform.up))
        {
            walkPointSet = true;
            animator.SetBool("isMoving", true);
        }
    }

    void BurnEnemies()
    {
        Vector3 distanceFromSnowman = player.transform.position - rangeFinder.position;

        // Increases temperature based on how close player is to anomaly
        if (distanceFromSnowman.magnitude <= 30f)
        {
            player.tempGain = 150f / distanceFromSnowman.magnitude;
            extinguished = false;
        }

        // Removes the temperature gain once player moves out of range
        else if(!extinguished)
        {
            player.tempGain -= 150f / distanceFromSnowman.magnitude;
            extinguished = true;
        }
    }
}

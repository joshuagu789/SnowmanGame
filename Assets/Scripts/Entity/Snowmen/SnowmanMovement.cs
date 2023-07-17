using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanMovement : MonoBehaviour
{
    /* 
     * This script was copy and pasted from the EnemyMovement script with the following modifications:
     *  - Snowmen patrol around the player's radius
     *  - Snowmen do not chase after its targets if they're out of player radius
     *  - Snowmen stop when player stops
     * Later combine this with the EnemyMovement script to make a universal movement script for NPC's?
    */
    public Entity entity;
    public Player player;

    // For patrolling movement
    public Vector3 walkPoint;
    public float walkPointRange;
    public float maxIdleTime;

    [SerializeField]
    private bool tooFarFromPlayer = false;
    [SerializeField]
    //private bool returningToPlayer = false;   EXPERIMENTAL TO MAKE SNOWMAN CHANGE DESTINATION WHILE MOVING

    // Timer for being idle
    float timer;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CheckDistanceFromPlayer());
        if (entity.isLockedOn)
        {
            Pursuing();
        }
        else if (tooFarFromPlayer)
        {
            Patrolling();   // Set for snowman to patrol around player
        }
        else if (!player.animator.GetBool("isMoving"))
        {
            Idle();
        }
        else
        {
            Patrolling();
        }
    }

    // Periodically check if snowman is getting too far away from player
    IEnumerator CheckDistanceFromPlayer()
    {
        var distance = new Vector3(entity.transform.position.x - player.transform.position.x, 0f,
                                   entity.transform.position.z - player.transform.position.z);

        if (distance.magnitude > entity.leashRange)
        {
            tooFarFromPlayer = true;
        }
        else
        {
            tooFarFromPlayer = false;
        }

        yield return new WaitForSeconds(1f);
    }

    void Idle()
    {
        entity.isMoving = false;
        entity.animator.SetBool("isMoving", false);

        /*
        // Making entity.agent idle for a period of time
        if (entity.isIdle)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                entity.isIdle = false;

            // 50% probability of being idle if entity.agent isn't patrolling
        }
        else if (Random.value >= 0.5f && !entity.isMoving)
        {
            timer = Random.value * maxIdleTime;
            entity.isIdle = true;
            if (entity.animator != null)
            {
                entity.animator.SetBool("isMoving", false);
            }
        }
        */
    }

    void Patrolling()
    {
        if (!entity.isMoving) //!entity.isMoving
        {
            SearchWalkPoint();
        }

        // If walkPoint is ready
        else if (entity.isMoving)
        {
            entity.agent.SetDestination(walkPoint);

            // Checking if destination is reached
            Vector3 distanceToWalkPoint = new Vector3(walkPoint.x - transform.position.x, 0f, walkPoint.z - transform.position.z);
            if (distanceToWalkPoint.magnitude < 1f)
            {
                entity.isMoving = false;
            }
        }
    }

    void SearchWalkPoint()
    {
        //Picking random coordinates to travel
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(player.transform.position.x + randomX, transform.position.y, player.transform.position.z + randomZ);

        // Checking if destination is within map     EXPERIMENTAL: have raycast go up as well if destination is at a higher
        //                                                          elevation (random coordinate assumes y position doesn't change)
        if (Physics.Raycast(walkPoint, -transform.up) || Physics.Raycast(walkPoint, -transform.up))
        {
            entity.isMoving = true;
            entity.animator.SetBool("isMoving", true);
        }
    }

    void Pursuing()
    {
        walkPoint = new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z);
        entity.isMoving = true;
        entity.animator.SetBool("isMoving", true);

        // Making entity.agent stop if in minimum range
        Vector3 distanceToWalkPoint = new Vector3(entity.target.position.x - transform.position.x, 0f, entity.target.position.z - transform.position.z);
        // walkPoint - transform.position;
        print("test");
        print(new Vector3(entity.target.position.x - transform.position.x, 0f, entity.target.position.z - transform.position.z).magnitude);

        if (distanceToWalkPoint.magnitude <= entity.minRange && !tooFarFromPlayer)
        {
            //entity.isMoving = false;
            //entity.animator.SetBool("isMoving", false);

            entity.agent.SetDestination(entity.transform.right * Random.Range(-5,5));
            Debug.Log("strafing");

            //entity.agent.isStopped = true;
        }
        else if (tooFarFromPlayer)
        {
            entity.isMoving = false;
            Patrolling();
        }
        // Making entity.agent move if out of minimum range
        else
        {
            entity.animator.SetBool("isMoving", true);

            //entity.agent.isStopped = false;
            // Making entity.agent chase after target
            entity.agent.SetDestination(walkPoint);     // Later make this a random point between min and max range?
        }
    }
}

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
    private bool walkPointSet = false;
    public float walkPointRange;
    public float maxIdleTime;

    private Vector3 strafingDirection;
    private bool strafingSet = false;

    [SerializeField]
    private bool tooFarFromPlayer = false;
    [SerializeField]
    private bool returningToPlayer = false;   //EXPERIMENTAL TO MAKE SNOWMAN CHANGE DESTINATION WHILE MOVING

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
        if (tooFarFromPlayer)
        {
            StartCoroutine(ReturnToPlayer());
        }
        else if (entity.isLockedOn && entity.target != null)
        {
            FaceTarget();
            Pursuing();
        }
        else if (!player.animator.GetBool("isMoving"))
        {
            Idle();
        }
        else
        {
            returningToPlayer = false;
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
            returningToPlayer = false;  // Means that entity is currently not approaching player
        }
        else
        {
            tooFarFromPlayer = false;
        }

        yield return new WaitForSeconds(1f);
    }

    IEnumerator ReturnToPlayer()
    {
        if (!returningToPlayer)
        {
            walkPointSet = false;
            strafingSet = false;
            Patrolling();   // Set for snowman to patrol around player
        }

        yield return new WaitForSeconds(1f);
    }

    void Idle()
    {
        entity.isMoving = false;
        walkPointSet = false;
        entity.animator.SetBool("isMoving", false);
    }

    void Patrolling()
    {
        if (!walkPointSet && !returningToPlayer) //!entity.isMoving
        {
            SearchWalkPoint();
        }

        // If walkPoint is ready
        if (walkPointSet)
        {
            entity.agent.SetDestination(walkPoint);
            returningToPlayer = true;

            // Checking if destination is reached
            Vector3 distanceToWalkPoint = new Vector3(walkPoint.x - transform.position.x, 0f, walkPoint.z - transform.position.z);
            if (distanceToWalkPoint.magnitude < 1f)
            {
                entity.isMoving = false;
                walkPointSet = false;
                returningToPlayer = false;
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
            walkPointSet = true;
            returningToPlayer = true;
            entity.animator.SetBool("isMoving", true);
        }
    }

    void Pursuing()
    {
        walkPoint = new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z);
        entity.isMoving = true;
        entity.animator.SetBool("isMoving", true);

        Vector3 distanceToWalkPoint = new Vector3(entity.target.position.x - transform.position.x, 0f, entity.target.position.z - transform.position.z);

        // Entity will strafe in directions other than forward if target is within minimum range
        if (distanceToWalkPoint.magnitude <= entity.minRange && !tooFarFromPlayer)
        {
            StrafeTarget(distanceToWalkPoint);
        }
        else if (tooFarFromPlayer)
        {
            Patrolling();
        }
        // Making entity.agent move if out of minimum range
        else if(distanceToWalkPoint.magnitude > entity.maxRange)
        {
            entity.animator.SetBool("isMoving", true);

            // Making entity.agent chase after target
            entity.agent.SetDestination(walkPoint);     // Later make this a random point between min and max range?
        }
    }

    private void FaceTarget()
    {
        // Swivelling game object to face target
        var targetRotation = Quaternion.LookRotation(new Vector3(entity.target.position.x - entity.transform.position.x,
                                                    entity.transform.position.y, entity.target.position.z - entity.transform.position.z));
        entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, targetRotation,     // Mathf.PI/180f since rotationSpeed is in degrees
                                                     entity.rotationSpeed * Time.deltaTime);    //  * Mathf.PI / 180f 
    }

    private void StrafeTarget(Vector3 distanceToTarget)
    {
        if (!strafingSet)
        {
            strafingDirection = Quaternion.AngleAxis(Random.Range(90f, 270f), entity.transform.up) * distanceToTarget.normalized * -1f * 5f;
            strafingSet = true;
        }
        else if (strafingSet)
        {
            entity.agent.SetDestination(strafingDirection);
            if (entity.agent.remainingDistance <= 1f)
            {
                strafingSet = false;
            }
        }
        Debug.Log("strafing");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Originally copy and pasted from script AnomalyAI.cs with the Pursuing() and FaceTarget() method added on- very similar to SnowmanMovement.cs
 *  - Later create a second script for BasicMovement called SmartMovement?
 */

public class BasicMovement : MonoBehaviour
{
    public Entity entity;

    // For patrolling movement
    public Vector3 walkPoint;
    public float walkPointRange;
    public float maxIdleTime;

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
        if (!entity.isLockedOn)
        {
            Idle();
            Patrolling();
        }
        else if (entity.isLockedOn)
        {
            entity.isIdle = false;
            Pursuing();
        }
    }

    void Idle()
    {
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
    }

    void Patrolling()
    {
        if (!entity.isMoving && !entity.isIdle)
        {
            SearchWalkPoint();
        }

        // If walkPoint is ready
        else if (entity.isMoving)
        {
            entity.agent.SetDestination(walkPoint);

            // Checking if destination is reached
            Vector3 distanceToWalkPoint = walkPoint - transform.position;
            if (distanceToWalkPoint.magnitude < 0.25f)
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
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Checking if destination is within map     EXPERIMENTAL: have raycast go up as well if destination is at a higher
        //                                                          elevation (random coordinate assumes y position doesn't change)
        if (Physics.Raycast(walkPoint, -transform.up) || Physics.Raycast(walkPoint, -transform.up))
        {
            entity.isMoving = true;
            if (entity.animator != null)
            {
                entity.animator.SetBool("isMoving", true);
            }
        }
    }

    void Pursuing()
    {
        walkPoint = new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z);
        entity.isMoving = true;

        if (entity.animator != null)
        {
            entity.animator.SetBool("isMoving", true);
        }

        // Making entity.agent chase after target
        entity.agent.SetDestination(walkPoint);     // Later make this a random point between min and max range?

        // Making entity.agent stop if in minimum range 
        Vector3 distanceToWalkPoint = new Vector3(walkPoint.x - transform.position.x, 0f, walkPoint.z - transform.position.z);
        FaceTarget();

        if (distanceToWalkPoint.magnitude < entity.minRange)
        {
            entity.isMoving = false;
            if (entity.animator != null)
            {
                entity.animator.SetBool("isMoving", false);
            }
            entity.agent.isStopped = true;
        }
        // Making entity.agent move if out of minimum range
        else 
        {
            if (entity.animator != null)
            {
                entity.animator.SetBool("isMoving", true);
            }
            entity.agent.isStopped = false;
        }
    }

    private void FaceTarget()
    {
        // Swivelling game object to face target
        var targetRotation = Quaternion.LookRotation(new Vector3(entity.target.position.x - entity.transform.position.x,
                                                    entity.transform.position.y, entity.target.position.z - entity.transform.position.z));
        entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, targetRotation,     // Converting rotationSpeed to radians just to make rotating slower
                                                     entity.rotationSpeed * Mathf.PI / 180f * Time.deltaTime);      // (Slerp's rotation speed value usually zero to one)
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Similar to snowman movement except there is no following a leader
 *  - Also similar to basic movement but with strafing both forward and backwards
 */

public class SmartMovement : MonoBehaviour
{
    public Entity entity;

    // For patrolling movement
    public Vector3 walkPoint;
    public float walkPointRange;
    public float maxIdleTime;

    // Timer for being idle
    float timer;

    public float strafeDistance;
    private Vector3 strafingDirection;  // Later replace this with walkPoint since they both do the same thing?
    private bool strafingSet = false;

    // Start is called before the first frame update
    void Start()
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
                entity.animator.Play("ForwardWalk");
            }
        }
    }

    void Pursuing()
    {
        walkPoint = new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z);
        entity.isMoving = true;
        entity.animator.SetBool("isMoving", true);

        Vector3 distanceToWalkPoint = new Vector3(entity.target.position.x - transform.position.x, 0f, entity.target.position.z - transform.position.z);

        // Entity will strafe in directions other than forward if target is within minimum range
        if (distanceToWalkPoint.magnitude <= entity.minRange)
        {
            entity.animator.Play("BackwardWalk");
            StrafeTarget(distanceToWalkPoint, "Backward");
            FaceTarget();
        }
        // Entity will strafe towards target once within max range (min and max range determines when entity will strafe)
        else if (distanceToWalkPoint.magnitude <= entity.maxRange)
        {
            entity.animator.Play("ForwardWalk");
            StrafeTarget(distanceToWalkPoint, "Forward");
            FaceTarget();
        }
        else
        {
            entity.animator.Play("ForwardWalk");
            entity.agent.SetDestination(walkPoint); // Entity heads directly towards target if out of max range
        }
    }

    private void StrafeTarget(Vector3 distanceToTarget, string direction)
    {
        if (!strafingSet)
        {
            if (direction.Equals("Forward"))
            {
                // Sets a destination between angles -90 & 90 for entity to travel               
                strafingDirection = Quaternion.AngleAxis(Random.Range(-90f, 90f), entity.transform.up) * distanceToTarget.normalized * strafeDistance;
            }
            else if (direction.Equals("Backward"))
            {
                // Sets a destination between angles 90 & 270 for entity to travel               multiply this by -1 so that AngleAxis rotations is applied backwards
                strafingDirection = Quaternion.AngleAxis(Random.Range(90f, 270f), entity.transform.up) * distanceToTarget.normalized * -1f * strafeDistance;
            }

            strafingSet = true;
        }
        else if (strafingSet)
        {
            // Making entity travel towards strafing destination
            entity.agent.SetDestination(strafingDirection);
            if (entity.agent.remainingDistance <= 1f)   // Resetting strafing once destination is reached
            {
                strafingSet = false;
            }
        }
    }

    private void FaceTarget()
    {
        // Swivelling game object to face target
        var targetRotation = Quaternion.LookRotation(new Vector3(entity.target.position.x - entity.transform.position.x,
                                                    entity.transform.position.y, entity.target.position.z - entity.transform.position.z));
        entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, targetRotation,    
                                                     entity.rotationSpeed / 10 * Time.deltaTime);      // (Slerp's rotation speed value usually zero to one)
                                                                        //but rotation speed needs to be high here to override NavMeshAgent's rotations
    }
}

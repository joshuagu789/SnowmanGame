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
    private float strafeAngle;

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
        else if (entity.isLockedOn && entity.target != null)
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

        }

        // 50% probability of being idle if entity.agent isn't patrolling
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
                //entity.animator.Play("ForwardWalk");
            }
        }
    }

    void Pursuing()
    {
        walkPoint = new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z);
        entity.isMoving = true;
        entity.animator.SetBool("isMoving", true);

        //Vector3 distanceToWalkPoint = new Vector3(entity.target.position.x - transform.position.x, 0f, entity.target.position.z - transform.position.z);

        // Entity will strafe in directions other than forward if target is within minimum range
        if (entity.distanceToTarget.magnitude <= entity.minRange)
        {
            StrafeTarget(90f, 270f);
            FaceTarget();
        }
        // Entity will strafe towards target once within max range (min and max range determines when entity will strafe)
        else if (entity.distanceToTarget.magnitude <= entity.maxRange)
        {
            StrafeTarget(0f, 360f);
            FaceTarget();
        }
        else 
        {
            //entity.animator.Play("ForwardWalk");
            strafingSet = false;
            FaceTarget();
            entity.agent.SetDestination(walkPoint); // Entity heads directly towards target if out of max range
        }
    }

    //private void StrafeTarget(Vector3 distanceToTarget, string direction)
    private void StrafeTarget(float minAngle, float maxAngle)
    {
        if (!strafingSet)
        {
            // Sets a destination between angles -90 & 90 for entity to travel
            strafeAngle = Random.Range(minAngle, maxAngle);
            strafingDirection = Quaternion.AngleAxis(strafeAngle, entity.transform.up) * entity.distanceToTarget.normalized * strafeDistance;

            strafingSet = true;
        }

        else if (strafingSet)
        {
            // Making entity move left/right/backwards based on angle
            if (strafeAngle >= 45 && strafeAngle <= 135)
                entity.animator.SetTrigger("RightWalk");
            else if(strafeAngle >= 135 && strafeAngle <= 225)
                entity.animator.SetTrigger("BackWalk");
            else if(strafeAngle <= -45 || strafeAngle >= 225 && strafeAngle <= 315)
                entity.animator.SetTrigger("LeftWalk");

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

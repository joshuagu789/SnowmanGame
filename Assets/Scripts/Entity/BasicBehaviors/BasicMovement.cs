using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Similar to SmartMovement.cs except instead of strafing this script can only have the entity move forward and backward
 *  - Also has different movement style if entity has leader like SmartMovement.cs
 */

public class BasicMovement : MonoBehaviour
{
    public Entity entity;

    // For patrolling movement
    public float walkPointRange;
    public float maxIdleTime;

    // Timer for being idle
    float timer;

    public float strafeDistance;    // Distance that entity will back up every time- if zero then entity will remain stationary if within minRange
    private Vector3 strafingDirection;  // Later replace this with walkPoint since they both do the same thing?
    private bool strafingSet = false;

    private bool tooFarFromLeader;
    private float timer1 = 1;   // Timer for CheckDistanceFromLeader()
    private float timer2 = 1;   // Timer for ReturnToLeader()


    // Update is called once per frame
    void Update()
    {
        if (entity.isLockedOn && entity.target != null && !entity.isDisabled)
            FaceTarget();
        // Specialized movement of entity if it has a leader (follows leader before attacking & patrolling on its own)
        if (entity.leader != null && !entity.isDisabled)
        {
            CheckDistanceFromLeader();
            if (tooFarFromLeader)
                ReturnToLeader();   // Executes every second- returning to leader always entity's highest priority
            else if (entity.isLockedOn && entity.target != null)    // Entity enters combat movement
            {
                entity.isIdle = false;
                Pursuing();
            }
            else if (entity.leader.GetComponent<Entity>().isIdle)   // Entity stops when it finishes current path and when leader stops
            {
                entity.isIdle = true;
                entity.walkPointSet = false;
                if (entity.agent.remainingDistance <= entity.agent.radius)
                {
                    entity.animator.SetBool("isMoving", false);
                    entity.agent.ResetPath();
                }
            }
            else
                Patrolling(entity.leader);
        }
        // How the entity normally moves and behaves without a leader (patrol around until target spotted- then attack)
        else if (!entity.isDisabled)
        {
            if (!entity.isLockedOn)
            {
                Idle();
                Patrolling(null);
            }
            else if (entity.isLockedOn && entity.target != null)
            {
                entity.isIdle = false;
                Pursuing();
            }
        }
    }

    // Periodically check if entity is getting too far away from leader
    void CheckDistanceFromLeader()
    {
        timer1 += Time.deltaTime;
        if (timer1 >= 1f)   // Calculates distance every 1f second to save up cpu
        {
            timer1 = 0;
            var distance = new Vector3(entity.leader.position.x - entity.transform.position.x, 0f,
                           entity.leader.position.z - entity.transform.position.z);

            if (distance.sqrMagnitude > entity.leashRange * entity.leashRange)
                tooFarFromLeader = true;
            else
                tooFarFromLeader = false;
        }
    }

    void ReturnToLeader()
    {
        timer2 += Time.deltaTime;

        // Cancelling current path entity is taking if shortest angle between entity's front and leader >= 45 degrees
        // so that entity sets down new walkpoint that's closer to leader
        if (timer2 >= 1f)
        {
            timer2 = 0;
            var angleToLeader = Vector3.Angle(transform.forward, new Vector3(entity.leader.position.x - entity.transform.position.x, 0f,
                                                                entity.leader.position.z - entity.transform.position.z));
            if (Mathf.Abs(angleToLeader) >= 45f)
                entity.walkPointSet = false;
        }

        entity.isIdle = false;
        strafingSet = false;
        Patrolling(entity.leader);   // Set for entity to patrol around player
    }

    void Idle()
    {
        // Making entity.agent idle for a period of time
        if (entity.isIdle)
        {
            timer -= Time.deltaTime;
            entity.walkPointSet = false;
            if (timer <= 0)
                entity.isIdle = false;
        }

        // 50% probability of being idle if entity.agent isn't patrolling
        else if (Random.value >= 0.5f && !entity.walkPointSet)
        {
            timer = Random.value * maxIdleTime;
            entity.isIdle = true;
            entity.animator.SetBool("isMoving", false);
        }
    }

    void Patrolling(Transform leader)
    {
        if (!entity.walkPointSet && !entity.isIdle)
            SearchWalkPoint(leader);

        // If walkPoint is ready
        else if (entity.walkPointSet && !entity.isIdle)
        {
            entity.agent.SetDestination(entity.GetWalkPoint());
            entity.animator.SetBool("isMoving", true);

            // Checking if destination is reached
            Vector3 distanceToWalkPoint = new Vector3(entity.GetWalkPoint().x - transform.position.x, 0f, entity.GetWalkPoint().z - transform.position.z);
            if (distanceToWalkPoint.sqrMagnitude < entity.agent.radius * entity.agent.radius)
                entity.walkPointSet = false;
        }
    }

    void SearchWalkPoint(Transform leader)
    {
        Vector3 walkPoint;

        // Entity moves around the leader's walk point range if it has one and moves around its own walk point range if not
        if (leader == null)
        {
            //Picking random coordinates to travel
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        }
        else
        {
            //Picking random coordinates to travel
            float randomZ = Random.Range(-entity.leashRange, entity.leashRange);
            float randomX = Random.Range(-entity.leashRange, entity.leashRange);

            walkPoint = new Vector3(leader.position.x + randomX, transform.position.y, leader.position.z + randomZ);
        }

        // Checking if destination is within map     EXPERIMENTAL: have raycast go up as well if destination is at a higher
        //                                                          elevation (random coordinate assumes y position doesn't change)
        if (Physics.Raycast(walkPoint + Vector3.up * 1000, Vector3.down) || Physics.Raycast(walkPoint + Vector3.down * 1000, Vector3.up))
            entity.MoveTo(walkPoint);
    }

    void Pursuing()
    {
        entity.MoveTo(new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z));
        entity.animator.SetBool("isMoving", true);

        // Entity will strafe in directions other than forward if target is within minimum range
        if (entity.distanceToTargetSqr <= entity.minRange * entity.minRange)
            Backpetal();
        else
        {
            strafingSet = false;
            entity.agent.SetDestination(new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z)); // Entity heads directly towards target if out of max range
        }
    }

    private void Backpetal()
    {
        if (!strafingSet)
        {
            strafingDirection = -entity.vectorToTarget.normalized * strafeDistance;

            strafingSet = true;
        }

        else if (strafingSet)
        {
            entity.animator.SetTrigger("BackWalk");

            // Making entity travel towards strafing destination
            entity.agent.SetDestination(entity.transform.position + strafingDirection);
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
                                                    0f, entity.target.position.z - entity.transform.position.z));
        entity.transform.rotation = Quaternion.Slerp(entity.transform.rotation, targetRotation,
                                                     entity.rotationSpeed / 10 * Time.deltaTime);      // (Slerp's rotation speed value usually zero to one)
                                                                                                       //but rotation speed needs to be high here to override NavMeshAgent's rotations
    }
}

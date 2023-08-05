using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * A combination of SnowmanMovement.cs and and BasicMovement.cs (which originally used exclusively for enemies)
 *  - Script is now universal and can be used for both enemies and allies
 *  - Features movement for staying near leader and also movement for acting on its own
 *  - Can patrol, be idle, stay near leader (if there is one), and strafe target 360 degrees
 */

public class SmartMovement : MonoBehaviour
{
    public Entity entity;

    // For patrolling movement
    public Vector3 walkPoint;
    private bool walkPointSet = false;
    public float walkPointRange;
    public float maxIdleTime;

    // Timer for being idle
    float timer;

    public float strafeDistance;
    private Vector3 strafingDirection;  // Later replace this with walkPoint since they both do the same thing?
    private bool strafingSet = false;
    private float strafeAngle;

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
            else 
            {
                /*
                 * If and else section below for being idle if leader is idle- had to have two sections for player leader and entity leader
                 * since player uses Player script to store isMoving bool and entity uses Entity script
                 */
                if (entity.leader.tag.Equals("Player"))
                {
                    var animator = entity.leader.gameObject.GetComponent<Player>().animator;
                    if (!animator.GetBool("isMoving"))
                    {
                        entity.isMoving = false;
                        entity.isIdle = true;
                        walkPointSet = false;
                        entity.agent.ResetPath();
                        entity.animator.SetBool("isMoving", false);
                    }
                    else
                        Patrolling(entity.leader);  // Wandering around within leash range of leader
                }
                else
                {
                    var animator = entity.leader.gameObject.GetComponent<Entity>().animator;
                    if (!animator.GetBool("isMoving"))
                    {
                        entity.isMoving = false;
                        entity.isIdle = true;
                        walkPointSet = false;
                        entity.agent.ResetPath();
                        entity.animator.SetBool("isMoving", false);
                    }
                    else
                        Patrolling(entity.leader);  // Wandering around within leash range of leader
                }
            }
        }
        // How the entity normally moves and behaves without a leader (patrol around until target spotted- then attack)
        else if(!entity.isDisabled)
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

            if (distance.magnitude > entity.leashRange)
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
                walkPointSet = false;
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
            if (timer <= 0)
                entity.isIdle = false;
        }

        // 50% probability of being idle if entity.agent isn't patrolling
        else if (Random.value >= 0.5f && !entity.isMoving)
        {
            timer = Random.value * maxIdleTime;
            entity.isIdle = true;
            entity.isMoving = false;
            walkPointSet = false;
            entity.animator.SetBool("isMoving", false);
        }
    }

    void Patrolling(Transform leader)
    {
        if (!walkPointSet && !entity.isIdle)
            SearchWalkPoint(leader);

        // If walkPoint is ready
        else if (walkPointSet)
        {
            entity.agent.SetDestination(walkPoint);

            // Checking if destination is reached
            Vector3 distanceToWalkPoint = new Vector3(walkPoint.x - transform.position.x, 0f, walkPoint.z - transform.position.z);
            if (distanceToWalkPoint.magnitude < 1f)
            {
                entity.isMoving = false;
                walkPointSet = false;
            }
        }
    }

    void SearchWalkPoint(Transform leader)
    {

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
        if (Physics.Raycast(walkPoint, -transform.up) || Physics.Raycast(walkPoint, -transform.up))
        {
            entity.isMoving = true;
            walkPointSet = true;
            entity.animator.SetBool("isMoving", true);
        }
    }

    void Pursuing()
    {
        walkPoint = new Vector3(entity.target.position.x, entity.target.position.y, entity.target.position.z);
        entity.isMoving = true;
        entity.animator.SetBool("isMoving", true);

        // Entity will strafe in directions other than forward if target is within minimum range
        if (entity.distanceToTarget.magnitude <= entity.minRange)
            StrafeTarget(90f, 270f);
        // Entity will strafe towards target once within max range (min and max range determines when entity will strafe)
        else if (entity.distanceToTarget.magnitude <= entity.maxRange)
            StrafeTarget(0f, 360f);
        else 
        {
            strafingSet = false;
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
            // Making entity move left/right/backwards in animator based on angle
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

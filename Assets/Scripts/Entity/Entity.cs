using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    /*
       Universal script for any game character that isn't the player (npc)- this script only has
       info-carrying variables that other scripts will use
    */

    public GameServer server;
    public Register register;
    public Animator animator;
    public NavMeshAgent agent;
    public string type;     // Can be either "Enemy" or "Snowman"

    public float systemIntegrity;
    public float maxIntegrity;

    public float temperature;
    public float minTemperature;

    public float integrityRegen;
    public float integrityLoss;
    public float tempGain;
    public float tempLoss;

    public float speed;
    public float rotationSpeed;

    // For targeting & distance measuring
    public float detectionRange;
    public float range;     // Maximum distance from target from which entity can act
    public float minRange;      // Minimum distance from target that entity can be (kind of like tether range)
    public float maxRange;      // Maximum distance from target that entity can be
    public float leashRange;    // Maximum distance from leader that entity can be (usually for snowmen & player)
    public Transform target = null;

    // For the robot's current state
    public bool isMoving = false;
    public bool isIdle = false;
    public bool isLockedOn = false;
    public float lockDuration;

    // Start is called before the first frame update
    void Start()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;
    }
    /*
    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        UpdateLockState();
        if (systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateStats()
    {

        if (register.hasTakenDamage)
        {
            systemIntegrity -= register.damageTaken;
            register.hasTakenDamage = false;
        }

    }

    // To make target lock go away after a duration
    void UpdateLockState()
    {
        if (isLockedOn)
        {
            StartCoroutine(LockLifetime());
        }
    }

    IEnumerator LockLifetime()
    {
        yield return new WaitForSeconds(lockDuration);

        // Removing the lock and resetting robot's states if target is outside detection range so robot can resume patrolling/being idle
        if (target != null && (target.position - transform.position).magnitude > detectionRange)
        {
            isLockedOn = false;
            isIdle = false;
            isMoving = false;
        }
    }
    */
}
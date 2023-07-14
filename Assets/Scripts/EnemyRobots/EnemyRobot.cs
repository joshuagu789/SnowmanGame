using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRobot : MonoBehaviour
{
    public GameServer server;
    public Register register;
    public Animator animator;
    public NavMeshAgent agent;

    public float integrity;
    public float speed;
    public float rotationSpeed;

    // For targeting
    public float detectionRange;
    public float range;     // Maximum distance from target from which entity can act
    public float minRange;      // Minimum distance from target that entity can be (kind of like tether range)
    public float maxRange;      // Maximum distance from target that entity can be
    public Transform target = null;

    // For the robot's current state
    public bool isMoving = false;
    public bool isIdle = false;
    public bool isLockedOn = false;
    public float lockDuration;

    // Start is called before the first frame update
    void Start()
    {
        speed = agent.speed;
        rotationSpeed = agent.angularSpeed;
    }

    private void Awake()
    {
        server.enemiesList.Add(transform);
    }

    private void OnDisable()
    {
        server.enemiesList.Remove(transform);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        UpdateLockState();
        if (integrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateStats()
    {
        
        if (register.hasTakenDamage)
        {
            integrity -= register.damageTaken;
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
}

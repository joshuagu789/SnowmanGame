using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Interacts only with Entity script to manage the unique characteristics that robot entities have
   (such as exploding and dropping spare parts when defeated)
*/

public class Robot : Entity
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        AddToServer();
    }

    private void OnDisable()
    {
        RemoveFromServer();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isLockedOn & target != null)
        {
            UpdateVectors();
        }

        UpdateStats();
        UpdateLockState();
        if (systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateStats()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        if (register.hasTakenDamage)
        {
            systemIntegrity -= register.damageTaken;
            register.hasTakenDamage = false;
        }

    }

    // To make target lock go away after a duration
    private void UpdateLockState()
    {
        if (isLockedOn)
        {
            StartCoroutine(LockLifetime());
        }
    }

    private IEnumerator LockLifetime()
    {
        yield return new WaitForSeconds(lockDuration);

        // Removing the lock and resetting robot's states if target is outside detection range so robot can resume patrolling/being idle
        if (target != null && (target.position - transform.position).magnitude > detectionRange)
        {
            isLockedOn = false;
            isIdle = false;
            agent.ResetPath();
        }
    }
}

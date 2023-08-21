using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Interacts only with Entity script to manage the unique characteristics that robot entities have
   (such as exploding and dropping spare parts when defeated)
*/

public class Robot : Entity
{
    private bool readyToCheckLock = true;

    // Start is called before the first frame update
    void Start()
    {
        defaultLeashRange = leashRange;
    }

    private void OnEnable()
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
        if (target == null)
            isLockedOn = false;
        else
            animator.SetBool("isLockedOn", true);
        if (readyToCheckLock && isLockedOn && distanceToTargetSqr > detectionRange * detectionRange)
        {
            readyToCheckLock = false;
            StartCoroutine(LockLifetime(target));
        }
    }

    private IEnumerator LockLifetime(Transform expectedTarget)
    {
        yield return new WaitForSeconds(lockDuration);

        // Removing the lock and resetting states if target is outside detection range so entity can resume patrolling/being idle
        if (target != null && distanceToTargetSqr > detectionRange * detectionRange && expectedTarget == target) // If the target of time lockDuration ago is still locked on
        {
            vectorToTarget = new Vector3(0f, 0f, 0f);
            distanceToTargetSqr = 0;
            isLockedOn = false;
            target = null;
            readyToCheckLock = true;
            isIdle = false;
            agent.ResetPath();
        }
        else
            readyToCheckLock = true;
    }
}

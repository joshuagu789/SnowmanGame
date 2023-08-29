using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Interacts only with Entity script to manage the unique characteristics that robot entities have
   (such as exploding and dropping spare parts when defeated)
    - robots can also be repaired
*/

public class Robot : Entity
{
    public GameObject destroyedVersion;     // Robots spawn their broken pieces when destroyed
    public GameObject explosion;            // They also explode when destroyed
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

    public virtual void OnDisable()
    {
        RemoveFromServer();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateVectors();
        CheckDamage();
        UpdateStats();
        UpdateLockState();
        if (systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Keeping stats within the specified boundaries
    public void UpdateStats()
    {
        if (agent != null)
        {
            agent.speed = speed;
            agent.angularSpeed = rotationSpeed;
        }
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    // To make target lock go away after a duration
    public virtual void UpdateLockState()
    {
        if (target == null || !isLockedOn) 
        {
            isLockedOn = false;
            animator.SetBool("isLockedOn", false);
        }
        else
        {
            animator.SetBool("isLockedOn", true);
            isLockedOn = true;
            if (readyToCheckLock && distanceToTargetSqr > detectionRange * detectionRange)
            {
                readyToCheckLock = false;
                StartCoroutine(LockLifetime(target));
            }
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

    public override void OnDestroy()    // Replacing itself with a destroyed version of itself
    {
        base.OnDestroy();
        if (systemIntegrity <= 0)
        {
            if (destroyedVersion != null)
                Instantiate(destroyedVersion, transform.position, transform.rotation);
            if (explosion != null)
                Instantiate(explosion, transform.position, transform.rotation);
        }
    }

    public virtual void Repair(float amount)
    {
        systemIntegrity += amount;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
   Interacts only with Entity script to manage the unique characteristics that robot entities have
   (such as exploding and dropping spare parts when defeated)
*/

public class Robot : MonoBehaviour
{
    public Entity entity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if (entity.type.Equals("Enemy"))
        {
            entity.server.enemiesList.Add(transform);
        }
        else if (entity.type.Equals("Snowman"))
        {
            entity.server.snowmenList.Add(transform);
        }
    }

    private void OnDisable()
    {
        if (entity.type.Equals("Enemy"))
        {
            entity.server.enemiesList.Remove(transform);
        }
        else if (entity.type.Equals("Snowman"))
        {
            entity.server.snowmenList.Remove(transform);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateStats();
        UpdateLockState();
        if (entity.systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateStats()
    {

        if (entity.register.hasTakenDamage)
        {
            entity.systemIntegrity -= entity.register.damageTaken;
            entity.register.hasTakenDamage = false;
        }

    }

    // To make target lock go away after a duration
    private void UpdateLockState()
    {
        if (entity.isLockedOn)
        {
            StartCoroutine(LockLifetime());
        }
    }

    private IEnumerator LockLifetime()
    {
        yield return new WaitForSeconds(entity.lockDuration);

        // Removing the lock and resetting robot's states if target is outside detection range so robot can resume patrolling/being idle
        if (entity.target != null && (entity.target.position - transform.position).magnitude > entity.detectionRange)
        {
            entity.isLockedOn = false;
            entity.isIdle = false;
            entity.isMoving = false;
        }
    }
}

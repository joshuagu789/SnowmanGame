using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Universal script assigned to snowmen- can it replace Player script for the player?
 *  - NOTE: this script is heavily copy & pasted from Robot and Player script
*/

public class Snowman : MonoBehaviour
{
    /*
     * Universal script assigned to snowmen- can it replace Player script for the player?
     *  - NOTE: this script is heavily copy & pasted from Robot and Player script
    */

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
    void Update()
    {
        UpdateStats();
        UpdateLockState();
        CheckMelt();
    }

    void UpdateStats()
    {
        // Calculating how to change integrity (aka health) and temperature 
        float tempChange = entity.tempGain - entity.tempLoss;
        entity.temperature += tempChange * Time.deltaTime;
        entity.temperature = Mathf.Clamp(entity.temperature, entity.minTemperature, Mathf.Infinity);

        float integrityChange = entity.integrityRegen - entity.integrityLoss - entity.temperature / 10f;
        entity.systemIntegrity += integrityChange * Time.deltaTime;
        entity.systemIntegrity = Mathf.Clamp(entity.systemIntegrity, 0, entity.maxIntegrity);

        if (entity.register.hasTakenDamage)
        {
            entity.systemIntegrity -= entity.register.damageTaken;
            entity.register.hasTakenDamage = false;
        }

    }

    // To make target lock go away after a duration
    void UpdateLockState()
    {
        if (entity.target == null)
        {
            entity.isLockedOn = false;
        }
        if (entity.isLockedOn)
        {
            StartCoroutine(LockLifetime());
        }
    }

    IEnumerator LockLifetime()
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

    void CheckMelt()
    {
        if (entity.systemIntegrity <= 0f)
        {
            entity.animator.SetBool("isMelting", true);
            entity.speed = 1f;
        }
    }
}

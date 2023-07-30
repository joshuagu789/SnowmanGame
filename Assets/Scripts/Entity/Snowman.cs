using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Universal script assigned to snowmen- can it replace Player script for the player?
 *  - NOTE: this script is heavily copy & pasted from Robot and Player script
*/

public class Snowman : MonoBehaviour
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
        CheckMelt();
        if(entity.systemIntegrity > 0)
            RepairDamage();
    }

    private void UpdateStats()
    {
        // Calculating how to change integrity (aka health) based on temperature 
        if (entity.temperature > entity.minTemperature)
            entity.systemIntegrity -= entity.temperature / 10f * Time.deltaTime;

        if (entity.register.hasTakenDamage)
        {
            entity.systemIntegrity -= entity.register.damageTaken;
            entity.temperature += entity.register.tempModifier;
            entity.register.hasTakenDamage = false;
        }

        ClampStats();
    }

    // Keeping stats within the specified boundaries
    private void ClampStats()
    {
        entity.temperature = Mathf.Clamp(entity.temperature, entity.minTemperature, Mathf.Infinity);
        entity.systemIntegrity = Mathf.Clamp(entity.systemIntegrity, 0, entity.maxIntegrity);
        entity.energy = Mathf.Clamp(entity.energy, 0, entity.maxEnergy);
    }

    // To make target lock go away after a duration
    private void UpdateLockState()
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

    private IEnumerator LockLifetime()
    {
        yield return new WaitForSeconds(entity.lockDuration);

        // Removing the lock and resetting states if target is outside detection range so robot can resume patrolling/being idle
        if (entity.target != null && (entity.target.position - transform.position).magnitude > entity.detectionRange)
        {
            entity.isLockedOn = false;
            entity.isIdle = false;
            entity.isMoving = false;
        }
    }

    private void CheckMelt()
    {
        if (entity.systemIntegrity <= 0f)
        {
            entity.animator.SetBool("isMelting", true);
            entity.speed = 1f;
        }
    }

    // Math formulas for converting energy into healh (aka integrity) and temperature repairs
    private void RepairDamage()
    {
        // Drains energy quickly to repair if health falls below threshold
        if (entity.systemIntegrity < entity.maxIntegrity / 3 && entity.energy > 0)
        {
            entity.systemIntegrity += entity.maxEnergy/10 * Time.deltaTime;
            entity.energy -= entity.maxEnergy / 10 * Time.deltaTime;
        }
        // Drains energy at log base 10 pace to repair if health still high
        else if (entity.systemIntegrity < entity.maxIntegrity && entity.energy > 0)
        {
            entity.systemIntegrity += Mathf.Log10(entity.maxIntegrity - entity.systemIntegrity) * Time.deltaTime;
            entity.energy -= Mathf.Log10(entity.maxIntegrity - entity.systemIntegrity) * Time.deltaTime;
        }
        // Rate of temp repair increases as temperature increases
        if (entity.temperature > entity.minTemperature && entity.energy > 0)
        {
            entity.temperature -= Mathf.Pow(1.5f, (entity.temperature - entity.minTemperature))/15f * Time.deltaTime;
        }
    }
}

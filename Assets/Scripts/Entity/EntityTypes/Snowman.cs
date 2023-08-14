using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Universal script assigned to snowmen- can it replace Player script for the player?
 *  - NOTE: this script is heavily copy & pasted from Robot and Player script
*/

public class Snowman : Entity
{
    public float minTemperature;
    private bool readyToCheckLock = true;
    private float lockTimer;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        defaultLeashRange = leashRange;
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
        CheckMelt();
        if(systemIntegrity > 0)
            RepairDamage();
    }

    private void UpdateStats()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        // Calculating how to change integrity (aka health) based on temperature 
        if (temperature > minTemperature)
            systemIntegrity -= temperature / 10f * Time.deltaTime;

        if (register.hasTakenDamage)
        {
            systemIntegrity -= register.damageTaken;
            temperature += register.tempModifier;
            register.hasTakenDamage = false;
        }

        ClampStats();
    }

    // Keeping stats within the specified boundaries
    public void ClampStats()
    {
        temperature = Mathf.Clamp(temperature, minTemperature, Mathf.Infinity);
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    // To make target lock go away after a duration
    private void UpdateLockState()
    {
        if (target == null)
            isLockedOn = false;
        if (readyToCheckLock && isLockedOn && distanceToTarget.sqrMagnitude > detectionRange * detectionRange)
        {
            readyToCheckLock = false;
            lockTimer = 0;
            LockLifetime(target);
        }
    }

    private void LockLifetime(Transform expectedTarget)
    {
        lockTimer += Time.deltaTime;
        // Removing the lock and resetting states if target is outside detection range so entity can resume patrolling/being idle
        if (lockTimer >= lockDuration && target != null && distanceToTarget.sqrMagnitude > detectionRange * detectionRange && expectedTarget == target) // If the target of time lockDuration ago is still locked on
        {
            distanceToTarget = new Vector3(0f, 0f, 0f);
            isLockedOn = false;
            target = null;
            readyToCheckLock = true;
            isIdle = false;
            agent.ResetPath();
        }
        else
            readyToCheckLock = true;
    }

    public void CheckMelt()
    {
        if (systemIntegrity <= 0f)
        {
            animator.SetBool("isMelting", true);
            speed = 1f;
        }
    }

    // Math formulas for converting energy into healh (aka integrity) and temperature repairs
    public void RepairDamage()
    {
        // Drains energy quickly to repair if health falls below threshold
        if (systemIntegrity < maxIntegrity / 3 && energy > 0)
        {
            systemIntegrity += maxEnergy/10 * Time.deltaTime;
            energy -= maxEnergy / 10 * Time.deltaTime;
        }
        // Drains energy at log base 10 pace to repair if health still high
        else if (systemIntegrity < maxIntegrity && energy > 0)
        {
            systemIntegrity += Mathf.Log10(maxIntegrity - systemIntegrity) * Time.deltaTime;    // Ratio of 2 integrity/health for 1 energy- 
            energy -= Mathf.Log10(maxIntegrity - systemIntegrity) * 0.5f * Time.deltaTime;      // means repairing at high health more energy efficient
        }
        // Rate of temp repair increases as temperature increases
        if (temperature > minTemperature && energy > 0)
        {
            temperature -= Mathf.Pow(1.25f, (temperature - minTemperature)/15f) * Time.deltaTime;
            energy -= Mathf.Pow(1.25f, (temperature - minTemperature)/15f) * Time.deltaTime;
        }
    }
}

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
    [SerializeField]
    private SnowmanCore core;
    private bool readyToCheckLock = true;

    private void OnEnable()
    {
        defaultLeashRange = leashRange;
        if (core != null)
            core.SetSnowmanPrefab(gameObject);
        AddToServer();
    }

    private void OnDisable()
    {
        RemoveFromServer();
    }

    public override void OnDestroy()    
    {
        base.OnDestroy();
        if (systemIntegrity <= 0)
            Instantiate(core.prefab, transform.position + Vector3.up, transform.rotation); // Dropping core when destroyed that allows for snowman's possible resurrection
    }

    // Update is called once per frame
    private void Update()
    {
        if (isLockedOn & target != null)
        {
            UpdateVectors();
        }

        CheckDamage();
        UpdateStats();
        UpdateLockState();
        if (systemIntegrity > 0)
            RepairDamage();
        else if (!animator.GetBool("isMelting"))
            Melt(); 
    }

    // Keeping stats within the specified boundaries
    public void UpdateStats()
    {
        // Calculating how to change integrity (aka health) based on temperature 
        if (temperature > minTemperature)
            systemIntegrity -= temperature / 10f * Time.deltaTime;

        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        temperature = Mathf.Clamp(temperature, minTemperature, Mathf.Infinity);
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    // To make target lock go away after a duration
    public override void UpdateLockState()
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

    public void Melt() { StartCoroutine(StartMelt()); }

    // Disabling and starting snowman's death animation then destroying game object is snowman is not saved by end of duration
    private IEnumerator StartMelt()
    {
        animator.SetBool("isMelting", true);
        isDisabled = true;
        speed /= 6;

        yield return new WaitForSeconds(20f);

        if (systemIntegrity > 0)    // If snowman has been saved
        {
            animator.SetBool("isMelting", false);
            yield return new WaitForSeconds(10f);
            isDisabled = false;
            speed *= 6;
        }
        else
            Destroy(gameObject);
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

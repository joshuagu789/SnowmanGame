using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Interacts only with Entity script to manage the unique characteristics that sunborn entities have
 *  - similar to robot script except no lock-on lifetime (will hunt target until one or the other is defeated)
 *  - similar to snowman script except higher temperature is good for sunborn while lower temperature harms it
 */

public class Sunborn : Entity
{
    public float maxTemperature;

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
        if (systemIntegrity <= 0)
            Destroy(gameObject);
        else if (systemIntegrity > 0)
            RepairDamage();
    }

    private void UpdateStats()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        if (register.hasTakenDamage)
        {
            animator.SetTrigger("Dodge");
            systemIntegrity -= register.damageTaken;
            temperature += register.tempModifier;
            register.hasTakenDamage = false;
        }
        ClampStats();
    }

    // Keeping stats within the specified boundaries
    private void ClampStats()
    {
        temperature = Mathf.Clamp(temperature, Mathf.NegativeInfinity, maxTemperature);
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    // Unlike Snowman.cs's RepairDamage(), this one repairs at a linear rate and can't repair temperature
    private void RepairDamage()
    {
        if(energy < maxEnergy)
            energy += temperature / 20 * Time.deltaTime;   // Sunborn enemies use temperature as a way to generate energy
        if (systemIntegrity < maxIntegrity && energy > 0)
        {
            systemIntegrity += maxEnergy / 20 * Time.deltaTime;
            energy -= maxEnergy / 20 * Time.deltaTime;
        }
    }
}

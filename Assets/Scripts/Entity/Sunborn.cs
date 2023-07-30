using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Interacts only with Entity script to manage the unique characteristics that sunborn entities have
 *  - similar to robot script except no lock-on lifetime (will hunt target until one or the other is defeated)
 *  - similar to snowman script except higher temperature is good for sunborn while lower temperature harms it
 */

public class Sunborn : MonoBehaviour
{
    public Entity entity;

    public float maxTemperature;

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
        if (entity.systemIntegrity <= 0)
            Destroy(gameObject);
        else if (entity.systemIntegrity > 0)
            RepairDamage();
    }

    private void UpdateStats()
    {
        if (entity.register.hasTakenDamage)
        {
            entity.animator.SetTrigger("Dodge");
            entity.systemIntegrity -= entity.register.damageTaken;
            entity.temperature += entity.register.tempModifier;
            entity.register.hasTakenDamage = false;
        }
        ClampStats();
    }

    // Keeping stats within the specified boundaries
    private void ClampStats()
    {
        entity.temperature = Mathf.Clamp(entity.temperature, Mathf.NegativeInfinity, maxTemperature);
        entity.systemIntegrity = Mathf.Clamp(entity.systemIntegrity, 0, entity.maxIntegrity);
        entity.energy = Mathf.Clamp(entity.energy, 0, entity.maxEnergy);
    }

    // Unlike Snowman.cs's RepairDamage(), this one repairs at a linear rate and can't repair temperature
    private void RepairDamage()
    {
        if(entity.energy < entity.maxEnergy)
            entity.energy += entity.temperature / 10 * Time.deltaTime;   // Sunborn enemies use temperature as a way to generate energy
        if (entity.systemIntegrity < entity.maxIntegrity && entity.energy > 0)
        {
            entity.systemIntegrity += entity.maxEnergy / 10f * Time.deltaTime;
            entity.energy -= entity.maxEnergy / 10f * Time.deltaTime;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child of Entity script to manage the unique characteristics that sunborn entities have 
/// <br/>
/// Typical traits: no lock-on lifetime (will hunt target until one or the other is defeated), higher temperature is good for sunborn while lower temperature harms it, dodge mechanic
/// </summary>
public class Sunborn : Entity
{
    public float maxTemperature; 
    public float deathTime;
    public float dodgeCooldown;

    private float cooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        defaultLeashRange = leashRange;
    }

    //private void OnEnable()
    //{
    //    //AddToServer();
    //}

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

        CheckDamage();
        UpdateStats();
        if (systemIntegrity <= 0)
            StartCoroutine(Die());
        else if (systemIntegrity > 0)
            RepairDamage();
        UpdateLockState();
    }

    public override void CheckDamage()
    {
        cooldownTimer += Time.deltaTime;

        if (register.HasTakenDamage() && cooldownTimer < dodgeCooldown) // If the sunborn took damage and isn't ready to dodge yet
        {
            // Makes entity target its attacker if the attack allows for it
            if (register.GetDamageSource() != null && !register.GetDamageSource().gameObject.GetComponentInParent<Entity>().GetEntityType().Equals(GetEntityType()))
            {
                FocusFire(register.GetDamageSource());
            }
            systemIntegrity -= register.GetDamageTaken();
            temperature += register.GetTempIncrease();
            register.ResetRegister();
        }
        else if (register.HasTakenDamage() && cooldownTimer >= dodgeCooldown  && !isDisabled)  // Sunborn can negate damage and play dodge animation if ready
        {
            cooldownTimer = 0;
            animator.SetTrigger("Dodge");
            register.ResetRegister();
        }
    }

    // Keeping stats within the specified boundaries
    private void UpdateStats()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        temperature = Mathf.Clamp(temperature, Mathf.NegativeInfinity, maxTemperature);
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);
        energy = Mathf.Clamp(energy, 0, maxEnergy);
    }

    private IEnumerator Die()
    {
        agent.isStopped = true;
        isDisabled = true;
        for (int i = 0; i < animator.layerCount; i++)   // Making entity play death animation across all of its animation layers
        {
            animator.Play("Death",i);
        }
        yield return new WaitForSeconds(deathTime);    // Destroying entity after death animation plays
        //DieNotPermanent();
        Destroy(gameObject);
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

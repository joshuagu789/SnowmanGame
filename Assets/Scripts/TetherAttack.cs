/*
 * Attack that steals/transfers one stat or another (temperature, energy, health aka integrity) once within range
 *  - becomes stronger the closer the target is
 *  - no cooldown since it is a continuous contact
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherAttack : MonoBehaviour
{
    public Entity entity;
    public Transform transferOrigin;

    public LineRenderer tetherBeam;
    public float integrityLoss;
    public float temperatureLoss;
    public float energyLoss;
    public float range;         
    public float fireAngleDeviation;

    public bool steals; // If true then attacker gains a resource while target loses it, if false then just make target lose resource

    // Update is called once per frame
    private void Update()
    {
        // Checking to see if the target meets requirements to be fired at 
        if (entity.target != null && entity.isLockedOn && entity.distanceToTarget.magnitude <= range && entity.angleToTarget <= (15 + fireAngleDeviation))
        {
            StartTether();
        }
        else
            tetherBeam.enabled = false;
    }

    private void StartTether()
    {
        // Creating tether effect
        tetherBeam.SetPosition(0, transferOrigin.position);
        tetherBeam.SetPosition(1, entity.target.position);
        tetherBeam.enabled = true;
        entity.animator.SetTrigger("Tether");

        var enemy = entity.target.GetComponent<Entity>();   // Accessing target's entity script
        enemy.systemIntegrity -= integrityLoss * Time.deltaTime;
        enemy.temperature -= temperatureLoss * Time.deltaTime;
        enemy.energy -= energyLoss * Time.deltaTime;

        if (steals) // Making entity gain the stats that target lost
        {
            entity.systemIntegrity += integrityLoss * Time.deltaTime;
            entity.temperature += temperatureLoss * Time.deltaTime;
            entity.energy += energyLoss * Time.deltaTime;
        }
    }
}

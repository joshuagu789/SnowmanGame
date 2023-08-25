/*
 * Similar to ArcingProjectileAttack but doesn't check for if the entity has a target or for range
 *  - kind of like blindly shooting in the hopes of the projectiles reaching the target
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntingProjectileAttack : ArcingProjectileAttack
{
    public float spotRadius;
    public bool movesToTarget;  // Whether the entity or not follows where its shots go when hunting

    private Vector3 possibleTargetPosition;
    private bool hasFoundTarget = false;

    private float targetTimer = 0;
    private float cooldowntimer = 0;

    // Update is called once per frame
    void Update()
    {
        cooldowntimer += Time.deltaTime;
        UpdateTarget();

        if (cooldowntimer >= cooldown && !entity.isLockedOn && !entity.isDisabled)
        {
            cooldowntimer = 0;

            if (isStationaryWhenFiring && entity.animator != null) // Making entity stop when firing
            {
                entity.animator.SetBool("isMoving", false);
                entity.agent.isStopped = true;
            }
            if (!hasFoundTarget)
                ShootRandomly();
            else
                ShootAtLocation(possibleTargetPosition);
        }
    }

    // Giving a lifetime to the time that entity has found a possible target which can only be extended by its projectiles reporting back targets
    // with ReportTarget(Vector3 location)
    private void UpdateTarget()
    {
        if (hasFoundTarget)
            targetTimer += Time.deltaTime;
        if (targetTimer >= cooldown * 2)
        {
            hasFoundTarget = false;
            possibleTargetPosition = new Vector3(0,0,0);
        }
    }

    private void ShootRandomly()
    {
        // Choosing a random location in range and in field of view for entity to blindly fire at
        var vectorToTarget = Quaternion.AngleAxis(Random.Range(-fireAngleDeviation, fireAngleDeviation), Vector3.up) * entity.transform.forward * range;
        ShootAtLocation(vectorToTarget);
    }

    public override void SpawnProjectile()
    {
        var attack = Instantiate(projectile, projectileOrigin.position, projectileOrigin.localRotation);        // Creating projectile

        // Setting the information of the projectile depending on if it can explode & deal damage or spot enemies or both
        if (attack.GetComponent<SpotterProjectile>() != null)
        {
            var attackScript = attack.GetComponent<SpotterProjectile>();
            attackScript.SetStats(spotRadius, entity.type, this);
        }
        if (attack.GetComponent<ExplosiveBullet>() != null)
        {
            var attackScript = attack.GetComponent<ExplosiveBullet>();
            attackScript.damage = damage;
            attackScript.explosionRadius = explosionRadius;
            attackScript.hasGravity = true;
        }

        // Applying initial velocity to make object fly as well as inaccuracy to the shot by rotating the vector to apply velocity with
        attack.GetComponent<Rigidbody>().velocity = Quaternion.AngleAxis(Random.Range(-fireAngleDeviation, fireAngleDeviation), Vector3.left) *
        Quaternion.AngleAxis(Random.Range(-fireAngleDeviation, fireAngleDeviation), Vector3.up) * projectileOrigin.forward * netVelocity;     
    }

    // If one of the fired projectiles has found an enemy
    public void ReportTarget(Vector3 location)
    {
        if(movesToTarget)
            entity.MoveTo(location);
        possibleTargetPosition = location;
        hasFoundTarget = true;
        targetTimer = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Causes entity to send X number of bullets in target direction if target is lined up with projectileOrigin
 *  - Experimental: takes into account number of attack animations entity has
 *  - A lot is copy and pasted from arcing projectile attack
 */

public class BulletAttack : Attack
{
    public Entity entity;
    public GameObject projectile;
    public Transform projectileOrigin; // Where the projectile will be created
    public GameObject spawnVFX;

    public int uniqueAttackNumber;    // If an entity has multiple unique attacks, they will be numbered from 1 to x
    public bool isStationaryWhenFiring;

    public float range;
    public float damage;
    public float explosionRadius;
    public float firingDelay;   // For projectile to appear with animation's timing

    public float bulletSpeed;
    public float bulletLifetime;
    public int bulletCount;       // Number of bullets fired per salvo/attack
    public float bulletInterval;    // Seconds between each bullet during a salvo/attack

    public float fireAngleDeviation;     // Max degrees that angle can deviate

    private int attackNumber;     
    private float cooldownTimer = 0;

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Checking to see if the target meets requirements to be fired at 
        if (entity.target != null && entity.isLockedOn && !entity.isDisabled)
        {
            // Checking to see if the target is in range, attack is off cooldown, and if target is in front
            if (entity.distanceToTargetSqr != 0 && entity.distanceToTargetSqr <= range * range && cooldownTimer > GetCooldown() && entity.angleToTarget <= (10 + fireAngleDeviation))
            {
                cooldownTimer = 0;
                StartCoroutine(Shoot());
            }
        }
    }

    IEnumerator Shoot()
    {
        attackNumber = (int)Random.Range(1, uniqueAttackNumber + 0.99f);    // Selecting random attack
        entity.animator.SetBool("isAttacking", true);
        entity.animator.SetTrigger("Attack" + attackNumber);

        yield return new WaitForSeconds(firingDelay);
        for (int x = 0; x < bulletCount; x++)
        {
            // Shooting one bullet and waiting a short delay until bulletCount number of bullets are fired
            SpawnBullets();
            yield return new WaitForSeconds(bulletInterval);
        }
        entity.animator.SetBool("isAttacking", false);
    }

    void SpawnBullets()
    {
        var bullet = Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation);
        // Creating VFX if the attack has one
        if(spawnVFX != null)
            Instantiate(spawnVFX, projectileOrigin.position, projectileOrigin.rotation);

        // Adding inaccuracy to shot by adjusting rotations by random range using var fireAngleDeviation
        bullet.transform.LookAt(entity.target);
        bullet.transform.Rotate(Random.Range(-fireAngleDeviation, fireAngleDeviation), projectileOrigin.localRotation.y +
                                                 Random.Range(-fireAngleDeviation, fireAngleDeviation), 0f);
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletSpeed;

        // Giving damage information to newly created bullet
        var bulletScript = bullet.GetComponent<ExplosiveBullet>();
        bulletScript.damage = damage;
        bulletScript.explosionRadius = explosionRadius;
        bulletScript.lifeTime = bulletLifetime;
        bulletScript.hasGravity = false;
    }
}

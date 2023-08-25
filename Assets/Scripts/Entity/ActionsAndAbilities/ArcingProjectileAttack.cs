using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * This script activates only if the Entity script has found a target within range. It then calculates the speed to fire
 * the projectile at using the kinematic equations of physics since one unit in Unity is equal to a meter. 
 */

public class ArcingProjectileAttack : MonoBehaviour
{

    public Entity entity;
    public GameObject projectile;
    public Transform projectileOrigin; // Where the projectile will be created

    public int uniqueAttackNumber;    // If an entity has multiple unique attacks, they will be numbered from 1 to x
    public bool isStationaryWhenFiring;

    public float damage;
    public float range;
    public float explosionRadius;
    public float cooldown;      // Time until entity can attack again
    public float firingDelay;   // For projectile to appear with animation's timing

    public int bulletCount;       // Number of bullets fired per salvo/attack
    public float bulletInterval;    // Seconds between each bullet during a salvo/attack

    public float fireAngle;        // Angle that projectile should be fired at
    public float fireAngleDeviation;     // Max degrees that angle can deviate

    private int attackNumber;
    private float timer = 0;
    [HideInInspector]
    public float netVelocity;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Checking to see if the target meets requirements to be fired at 
        if (entity.target != null && entity.isLockedOn && !entity.isDisabled)
        {
            // Checking to see if the target is in range, attack is off cooldown, and if target is in front
            if (entity.distanceToTargetSqr != 0 && entity.distanceToTargetSqr <= entity.range * entity.range && timer > cooldown && entity.angleToTarget <= (5 + fireAngleDeviation))
            {
                if (isStationaryWhenFiring && entity.animator != null) // Making entity stop when firing
                {
                    entity.animator.SetBool("isMoving", false);
                    entity.agent.isStopped = true;
                }
                timer = 0;
                StartCoroutine(Shoot(entity.vectorToTarget));
            }
        }
    }

    private void CalculateSpeed(float distance)
    {
        // From the formula Vx * time = distance aka Vcos(angle) * time = distance (Vx is horizontal component of velocity)
        // Along with Vyf = Vyi + at aka 0 = Vsin(angle) - 9.8t with t isolated and substituted into Vcos(angle) * time = distance
        // NOTE: t for vertical component is half of time of t of horizontal component
        print("distance: " + distance);
                                                // Offset by 1.4 factor bc equation assumes start and end are at same height but not in reality
        netVelocity = Mathf.Sqrt((float)((9.8f * (distance/1.4f)) / (2f * Mathf.Sin(fireAngle) * Mathf.Cos(fireAngle))));
        print("a");
    }

    public void ShootAtLocation(Vector3 vectorToLocation)
    {
        StartCoroutine(Shoot(vectorToLocation));
    }

    IEnumerator Shoot(Vector3 vectorToTarget)
    {
        attackNumber = (int)Random.Range(1, uniqueAttackNumber + 0.99f);    // Selecting random attack
        entity.animator.SetBool("isAttacking", true);
        entity.animator.SetTrigger("Attack" + attackNumber);

        yield return new WaitForSeconds(firingDelay);
        for (int x = 0; x < bulletCount; x++)
        {
            CalculateSpeed(vectorToTarget.magnitude);
            SpawnProjectile();
            yield return new WaitForSeconds(bulletInterval);
        }

        if (isStationaryWhenFiring)
            entity.agent.isStopped = false;
        entity.animator.SetBool("isAttacking", false);
    }

    public virtual void SpawnProjectile()
    {
        // Adding inaccuracy to shot by making projectileOrigin rotate a bit
        projectileOrigin.localRotation = Quaternion.Euler(-fireAngle + Random.Range(-fireAngleDeviation, fireAngleDeviation),
                                                  Random.Range(-fireAngleDeviation, fireAngleDeviation), 0f);

        var attack = Instantiate(projectile, projectileOrigin.position, projectileOrigin.localRotation);        // Creating projectile
        var attackScript = attack.GetComponent<ExplosiveBullet>();

        attack.GetComponent<Rigidbody>().velocity = netVelocity * projectileOrigin.forward;     // Applying initial velocity to make object fly
        attackScript.damage = damage;
        attackScript.explosionRadius = explosionRadius;
        attackScript.hasGravity = true;
    }
}

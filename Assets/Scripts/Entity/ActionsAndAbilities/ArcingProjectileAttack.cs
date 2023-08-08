using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * This script activates only if the Entity script has found a target within range. It then calculates the trajectory to fire
 * the projectile using the kinematic equations of physics since one unit in Unity is equal to a meter. 
 */

public class ArcingProjectileAttack : MonoBehaviour
{

    public Entity entity;
    public GameObject projectile;
    public Transform projectileOrigin; // Where the projectile will be created

    public bool isStationaryWhenFiring;

    public float damage;
    public float explosionRadius;
    public float cooldown;      // Time until entity can attack again
    public float firingDelay;   // For projectile to appear with animation's timing
    //public float horizontalVelocity;
    public float fireAngle;        // Angle that projectile should be fired at
    public float fireAngleDeviation;     // Max degrees that angle can deviate

    private float timer = 0;
    private float netVelocity;
    private Vector3 distanceToTarget;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Checking to see if the target meets requirements to be fired at 
        if (entity.target != null && entity.isLockedOn)
        {
            distanceToTarget = new Vector3(entity.target.position.x - projectileOrigin.position.x, 0f,
                                                    entity.target.position.z - projectileOrigin.position.z);
            var angleToTarget = Vector3.Angle(transform.forward, distanceToTarget);

            // Checking to see if the target is in range, attack is off cooldown, and if target is in front
            if (distanceToTarget.sqrMagnitude <= entity.range * entity.range && timer > cooldown && angleToTarget <= (20 + fireAngleDeviation))
            {
                if (isStationaryWhenFiring)
                {
                    // Robot can't move while firing
                    entity.animator.SetBool("isMoving", false);
                    entity.agent.isStopped = true;
                }

                entity.animator.SetBool("isAttacking", true);
                timer = 0;
                CalculateTrajectory(distanceToTarget);
                StartCoroutine(ShootArc());
            }
        }
    }

    private void CalculateTrajectory(Vector3 distance)
    {
        projectileOrigin.localRotation = Quaternion.Euler(-fireAngle + Random.Range(-fireAngleDeviation, fireAngleDeviation),
                                                          Random.Range(-fireAngleDeviation, fireAngleDeviation), 0f);

        // From the formula Vx * time = distance aka Vcos(angle) * time = distance (Vx is horizontal component of velocity)
        // Along with Vyf = Vyi + at aka 0 = Vsin(angle) - 9.8t with t isolated and substituted into Vcos(angle) * time = distance
        // NOTE: t for vertical component is half of time of t of horizontal component

                                                // Offset by 1.4 factor bc equation assumes start and end are at same height but not in reality
        netVelocity = Mathf.Sqrt((float)((9.8 * (distance.magnitude/1.4)) / (2 * Mathf.Sin(fireAngle) * Mathf.Cos(fireAngle))));
    }

    IEnumerator ShootArc()
    {
        yield return new WaitForSeconds(firingDelay);
        SpawnProjectile();
        if (isStationaryWhenFiring)
            entity.agent.isStopped = false;
    }

    private void SpawnProjectile()
    {
        var attack = Instantiate(projectile, projectileOrigin.position, projectileOrigin.localRotation);        // Creating projectile
        var attackScript = attack.GetComponent<ExplosiveBullet>();

        attack.GetComponent<Rigidbody>().velocity = netVelocity * projectileOrigin.forward;     // Applying initial velocity to make object fly
        attackScript.damage = damage;
        attackScript.explosionRadius = explosionRadius;
        attackScript.hasGravity = true;

        entity.animator.SetBool("isAttacking", false);

    }
}

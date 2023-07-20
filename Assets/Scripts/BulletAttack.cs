using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Causes entity to send X number of bullets in target direction if target is lined up with projectileOrigin
 *  - Experimental: takes into account number of attack animations entity has
 *  - A lot is copy and pasted from arcing projectile attack
 */

public class BulletAttack : MonoBehaviour
{
    public Entity entity;
    public GameObject projectile;
    public Transform projectileOrigin; // Where the projectile will be created

    public float uniqueAttackNumber;    // If an entity has multiple unique attacks, they will be numbered from 1 to x
    public bool isStationaryWhenFiring;

    public float damage;
    public float explosionRadius;
    public float cooldown;      // Time until entity can attack again
    public float firingDelay;   // For projectile to appear with animation's timing

    public float bulletSpeed;
    public float bulletLifetime;
    public float bulletCount;       // Number of bullets fired per salvo/attack
    public float bulletInterval;    // Seconds between each bullet during a salvo/attack

    public float fireAngleDeviation;     // Max degrees that angle can deviate

    private bool isAttacking;
    private int attackNumber;     
    private float delayTimer = 0;
    private float cooldownTimer = 0;
    private float intervalTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Checking to see if the target meets requirements to be fired at 
        if (entity.target != null && entity.isLockedOn)
        {
            var distanceToTarget = new Vector3(entity.target.position.x - projectileOrigin.position.x, 0f,
                                                    entity.target.position.z - projectileOrigin.position.z);
            var angleToTarget = Vector3.Angle(transform.forward, distanceToTarget);

            // Checking to see if the target is in range, attack is off cooldown, and if target is in front
            if (distanceToTarget.magnitude <= entity.range && cooldownTimer > cooldown && angleToTarget <= (10 + fireAngleDeviation))
            {
                cooldownTimer = 0;
                attackNumber = (int)Random.Range(1, uniqueAttackNumber + 0.99f);    // Selecting random attack
                isAttacking = true;
                //Shoot();
                entity.animator.SetTrigger("Attack" + attackNumber);
                StartCoroutine(Shoot());
            }
        }
        /*
        while (isAttacking)
        {
            entity.animator.SetTrigger("Attack" + attackNumber);
            delayTimer += Time.deltaTime;

            if (delayTimer >= firingDelay)
            {
                for (int i = 0; i < bulletCount; i++)
                {
                    StartCoroutine(SpawnBullets());
                    if (i == bulletCount - 1)
                    {
                        delayTimer = 0;
                        isAttacking = false;
                    }
                }
            }
        }
        */
    }

    /*
    private void Shoot()
    {
        
        while (isAttacking)
        {
            Debug.Log(delayTimer);

            entity.animator.SetTrigger("Attack" + attackNumber);
            delayTimer += Time.deltaTime;

            if (delayTimer >= firingDelay)
            {
                for (int i = 0; i < bulletCount; i++)
                {
                    StartCoroutine(SpawnBullets());
                    if (i == bulletCount - 1)
                    {
                        delayTimer = 0;
                        isAttacking = false;
                    }
                }
            }
        }
        
        entity.animator.SetTrigger("Attack" + attackNumber);

        for (float i = 0; i <= firingDelay + 1; i += Time.deltaTime)
        {
            if (i >= firingDelay)
            {
                for (int x = 0; x < bulletCount; x++)
                {
                    StartCoroutine(SpawnBullets());
                    if (i == bulletCount - 1)
                    {
                        delayTimer = 0;
                        isAttacking = false;
                    }
                }
                break;
            }
        }
    }
    */

    IEnumerator Shoot()
    {
        yield return new WaitForSeconds(firingDelay);
        for (int x = 0; x < bulletCount; x++)
        {
            SpawnBullets();
            yield return new WaitForSeconds(bulletInterval);
        }
        //SpawnBullets();
    }

    void SpawnBullets()
    {
        projectileOrigin.LookAt(new Vector3(0f, entity.target.position.y, 0f));

        // Adding inaccuracy to shot by adjusting rotations by random range using var fireAngleDeviation
        projectileOrigin.localRotation = Quaternion.Euler(Random.Range(-fireAngleDeviation, fireAngleDeviation), projectileOrigin.localRotation.y +
                                                  Random.Range(-fireAngleDeviation, fireAngleDeviation), 0f);

        // Giving damage information to newly created bullet
        var bullet = Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation);
        var bulletScript = bullet.GetComponent<ExplosiveBullet>();

        bulletScript.damage = damage;
        bulletScript.explosionRadius = explosionRadius;
        bulletScript.lifeTime = bulletLifetime;
        bulletScript.speed = bulletSpeed;
        bulletScript.hasGravity = false;
    }

    /*
    IEnumerator SpawnBullets()
    {
        Debug.Log("fired");
        projectileOrigin.LookAt(new Vector3(0f, entity.target.position.y, 0f));

        // Adding inaccuracy to shot by adjusting rotations by random range using var fireAngleDeviation
        projectileOrigin.localRotation = Quaternion.Euler(Random.Range(-fireAngleDeviation, fireAngleDeviation), projectileOrigin.localRotation.y +
                                                  Random.Range(-fireAngleDeviation, fireAngleDeviation), 0f);

        // Giving damage information to newly created bullet
        var bullet = Instantiate(projectile, projectileOrigin.position, projectileOrigin.localRotation);
        var bulletScript = bullet.GetComponent<ExplosiveBullet>();

        bulletScript.damage = damage;
        bulletScript.explosionRadius = explosionRadius;
        bulletScript.lifeTime = bulletLifetime;
        bulletScript.speed = bulletSpeed;
        bulletScript.hasGravity = false;

        yield return new WaitForSeconds(bulletInterval);
    }
    */
}

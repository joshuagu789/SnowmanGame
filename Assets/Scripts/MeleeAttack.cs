using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Close range attack where damage is applied directly to target's Register.cs (similar to TetherAttack.cs)
 */

public class MeleeAttack : MonoBehaviour
{
    public Entity entity;

    public float uniqueAttackNumber;    // If an entity has multiple unique attacks, they will be numbered from 1 to x
    public bool isStationaryWhenFiring;

    public float damage;
    public float explosionRadius;
    public float cooldown;      // Time until entity can attack again
    public float firingDelay;   // For projectile to appear with animation's timing
    public float range;

    public float bulletCount;       // Number of punches thrown per salvo/attack
    public float bulletInterval;    // Seconds between each melee during a salvo/attack

    public float fireAngleDeviation;     // Max degrees that angle can deviate

    private Entity enemy;
    private int attackNumber;
    private float cooldownTimer = 0;

    // Update is called once per frame
    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Checking to see if the target meets requirements to be fired at 
        if (entity.target != null && entity.isLockedOn && !entity.isDisabled)
        {
            // Checking to see if the target is in range, attack is off cooldown, and if target is in front
            if (entity.distanceToTarget.magnitude != 0 && entity.distanceToTarget.magnitude <= range && cooldownTimer > cooldown && entity.angleToTarget <= (15 + fireAngleDeviation))
            {
                if (isStationaryWhenFiring)
                {
                    entity.animator.SetBool("isMoving", false);
                    entity.agent.isStopped = true;
                }

                cooldownTimer = 0;
                attackNumber = (int)Random.Range(1, uniqueAttackNumber + 0.99f);    // Selecting random attack
                entity.isDisabled = true;
                entity.animator.SetBool("isAttacking", true);
                entity.animator.SetTrigger("Melee" + attackNumber);

                // Making target stop moving
                enemy = entity.target.GetComponent<Entity>();   
                enemy.animator.SetBool("isMoving", false);
                enemy.agent.isStopped = true;

                StartCoroutine(Melee());
            }
        }
    }

    private IEnumerator Melee()
    {
        yield return new WaitForSeconds(firingDelay);

        if (entity.target != null)
        {
            var enemyRegister = entity.target.GetComponentInChildren<Register>();

            for (int x = 0; x < bulletCount; x++)
            {
                enemyRegister.TakeDamage(transform, damage, 0);
                yield return new WaitForSeconds(bulletInterval);
            }
            entity.animator.SetBool("isAttacking", false);
            entity.isDisabled = false;
            if (isStationaryWhenFiring)
                entity.agent.isStopped = false;
            if (entity.target != null)
                enemy.agent.isStopped = false;
        }
        else    // If the target is destroyed before the attack executes
        {
            entity.animator.SetBool("isAttacking", false);
            entity.agent.isStopped = false;
            entity.isDisabled = false;
        }
    }
}

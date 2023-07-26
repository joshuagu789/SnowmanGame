using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * First weapon created that deals damage
 *  - unlike other weapons this one is not part of user's animation but added on as an independent turret-like weapon
 */

[RequireComponent(typeof(LineRenderer))]
public class IceBeamRig : MonoBehaviour
{
    // For animating laser beam
    Animator animator;
    bool toggleActive = false;

    // For laser beam
    public Player player;
    public PlayerTargeting PlayerTargeting;
    public Transform laserOrigin;
    LineRenderer laser;

    // For laser stats
    public float damage = 15f;
    public float tempModifier = -15f;
    public float weaponRange = 75f;
    public float laserDuration = 1f;
    public float recoil = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isAttacking", false);
        laser = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Includes actual firing of ice beam 
        CheckWeaponStates();
    }

    private void CheckWeaponStates()
    {
        // Re-arming ice beam after firing animation finishes
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ice Beam Rig Active"))
        {
            animator.SetBool("isAttacking", false);
        }

        // To make ready the ice beam
        if (Input.GetKeyDown(KeyCode.E))
        {
            toggleActive = !toggleActive;
            animator.SetBool("isActive", toggleActive);
        }
        // To shoot and play firing animation
        else if (Input.GetButtonDown("Fire1") && toggleActive && !animator.GetBool("isAttacking") &&
                 animator.GetCurrentAnimatorStateInfo(0).IsName("Ice Beam Rig Active"))
        {
            animator.SetBool("isAttacking", true);
            Shoot();
        }
    }

    // To shoot laser beam
    private void Shoot()
    {
        // Laser beam always hit target if player is locked on in PlayerTargeting script
        if (player.isLockedOn && player.target != null)
        {
            //if (Physics.Raycast(player.position, PlayerTargeting.target.position, weaponRange))
            if ((player.target.position - player.transform.position).magnitude <= weaponRange)
            {
                laser.SetPosition(0, laserOrigin.position);
                laser.SetPosition(1, player.target.position);

                StartCoroutine(ShootLaser());
            }
            else
            {
                //laser.SetPosition(1, laserOrigin.position * weaponRange);
            }
        }
    }

    // To produce laser effect for time laserDuration
    IEnumerator ShootLaser()
    {
        laser.enabled = true;

        // Making target take damage
        //player.target.gameObject.GetComponent<Register>().TakeDamage(transform, damage, tempModifier);
        player.target.gameObject.GetComponentInChildren<Register>().TakeDamage(transform, damage, tempModifier);

        player.speed /= recoil;
        yield return new WaitForSeconds(laserDuration);
        player.speed *= recoil;
        laser.enabled = false;
    }
}

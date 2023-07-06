using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(LineRenderer))]
public class IceBeamRig : MonoBehaviour
{
    // For animating laser beam
    Animator animator;
    bool toggleActive = false;

    // For laser beam
    public PlayerTargeting PlayerTargeting;
    public Transform laserOrigin;
    public Transform player;
    LineRenderer laser;
    
    public float weaponRange = 75f;
    public float laserDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("hasFired", false);
        laser = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Includes actual firing of ice beam 
        CheckWeaponStates();
    }

    void CheckWeaponStates()
    {
        // Re-arming ice beam after firing animation finishes
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ice Beam Rig Active"))
        {
            animator.SetBool("hasFired", false);
        }

        // To make ready the ice beam
        if (Input.GetKeyDown(KeyCode.E))
        {
            toggleActive = !toggleActive;
            animator.SetBool("isActive", toggleActive);
        }
        // To shoot and play firing animation
        else if (Input.GetButtonDown("Fire1") && toggleActive && !animator.GetBool("hasFired") &&
                 animator.GetCurrentAnimatorStateInfo(0).IsName("Ice Beam Rig Active"))
        {
            animator.SetBool("hasFired", true);
            Shoot();
        }
    }

    // To shoot laser beam
    void Shoot()
    {
        // Laser beam always hit target if player is locked on in PlayerTargeting script
        if (PlayerTargeting.isLockedOn)
        {
            //if (Physics.Raycast(player.position, PlayerTargeting.target.position, weaponRange))
            if ((PlayerTargeting.target.position - player.position).magnitude <= weaponRange)
            {
                Debug.Log("In range");
                laser.SetPosition(0, laserOrigin.position);
                laser.SetPosition(1, PlayerTargeting.target.position);
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
        yield return new WaitForSeconds(laserDuration);
        laser.enabled = false;
    }
}
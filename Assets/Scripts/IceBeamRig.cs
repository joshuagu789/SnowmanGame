using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class IceBeamRig : MonoBehaviour
{
    public Animator animator;

    bool toggleActive = false;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("hasFired", false);
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
        }
    }
}

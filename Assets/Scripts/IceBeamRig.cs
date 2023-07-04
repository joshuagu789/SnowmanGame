using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class IceBeamRig : MonoBehaviour
{
    public Animator animator;
    //public CinemachineVirtualCamera firstPersonCam;
    //public Transform targetLock;

    bool toggleActive = false;
    bool isLockedOn = false;
    //public float range = 100f;

    // Start is called before the first frame update
    void Start()
    {
        animator.SetBool("hasFired", false);
    }

    // Update is called once per frame
    void Update()
    {
        //CheckLockOn();
        // Includes actual firing of ice beam 
        CheckWeaponStates();
    }

    void CheckLockOn()
    {
         /*                   // Right click
        if (Input.GetButtonDown("Fire2") && firstPersonCam.enabled)
        {
            // Locking on target if it's in range
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the has hit an enemy
            if (Physics.Raycast(ray, out hit, range) && hit.collider.gameObject.CompareTag("Enemy"))
            {
                Debug.Log(hit.collider.gameObject);
                var target = Instantiate(targetLock, hit.collider.transform.position, hit.collider.transform.localRotation, hit.collider.transform);
            }
            
        }
         */
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

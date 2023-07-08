using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCam;
    public Transform targetLock;
    Player player;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLockState();
        if (player.isLockedOn)
        {
            AdjustRotation();
        }
    }

    // Managing the target lock
    void CheckLockState()
    {
        if (Input.GetButtonDown("Fire2") && firstPersonCam.enabled)
        {
            // Locking on target if it's in range
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // If the ray has hit an enemy within range
            if (Physics.Raycast(ray, out hit, player.targetingRange) && hit.collider.gameObject.CompareTag("Enemy"))
            {
                // To remove any pre-existing locks
                if (player.isLockedOn)
                {
                    Destroy(target.gameObject);
                }

                target = Instantiate(targetLock, hit.collider.transform.position, hit.collider.transform.localRotation, hit.collider.transform);
                target.localScale = hit.collider.transform.localScale;
                player.isLockedOn = true;
            }

        }
    }

    // Having the torso of player face the target if locked on (rotating torso also rotates attached weapons)
    void AdjustRotation()
    {
        // Retrieving torso with its parent's parent 
        Transform torso = transform.GetChild(0).GetChild(2);

        /*
            The below code can usually be done simply with LookAt() except because of Blender to Unity export issues the torso part has
            its y axis as its forward (but also backwards) so had to use Vector3.Angle() to find angle to swivel z axis to face target
            (z axis is up for torso bc of export issues)- the Angle() method also only does angles 0-180 degrees so InverseTransformPoint
            is used to store target's location as local to compare with player's position to see if it is left or right of player
        */

        var targetDir = new Vector3(target.transform.position.x - torso.transform.position.x,   // Vector3 representing direction between 
                            0f, target.transform.position.z - torso.transform.position.z).normalized;   // player and target

        float angle = Vector3.Angle(targetDir, player.transform.forward);   // Angle between two vectors

        Vector3 targetLocalPos = transform.InverseTransformPoint(target.position);  // Storing target's location relative to player

        if (targetLocalPos.x < 0)   // If target is left of player
        {
            torso.localRotation = Quaternion.Euler(270f, 0f, -angle);
        }
        else if (targetLocalPos.x > 0)  // If target is right of player
        {
            torso.localRotation = Quaternion.Euler(270f, 0f, angle);
        }
    }
}

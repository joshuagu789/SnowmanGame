using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

/* 
 * Currently requires player to be in first person view and right click repeatedly until target found by raycast
 *  - Later make it so that player can target lock closest enemy when right clicked in third person view?
 */

public class PlayerTargeting : MonoBehaviour
{

    public CameraController camera;
    public GameServer server;
    public Transform targetLock;
    Player player;
    private Transform targetLockClone;

    private Quaternion lookRotation;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLockState();
        // Resetting torso's rotation if there is no target lock
        if(!player.isLockedOn)
        {
            Transform torso = transform.Find("Armature").Find("Torso");
            torso.localRotation = Quaternion.Slerp(torso.localRotation, Quaternion.Euler(-90f, 0f, 0f), 2 * Time.deltaTime);
        }
    }

    private void LateUpdate()   // Rotating a bone that's in an animator to override it (LateUpdate calls after animations update)
    {
        if (player.isLockedOn && player.target != null)
        {
            AdjustRotation();
        }
    }

    // Managing the target lock
    void CheckLockState()
    {
        
        // To see if the target has been destroyed
        if (player.target == null)
        {
            player.isLockedOn = false;
            if (targetLockClone != null)
                Destroy(targetLockClone);
        }

        // Getting a target lock when player right clicks based on where they're looking 
        if (Input.GetButtonDown("Fire2") && camera.thirdPersonCam.enabled || Input.GetButtonDown("Fire2") && camera.firstPersonCam.enabled)
        {
            bool targetFound = false;
            Transform closestTarget = null;
            float minPriorityRank = Mathf.Infinity;

            // Looping through all enemies currently active 
            foreach (Transform potentialTarget in server.enemiesList)
            {
                Vector3 distance = new Vector3(potentialTarget.position.x - transform.position.x, 0f, potentialTarget.position.z - transform.position.z);
                float angle = Vector3.Angle(camera.transform.forward, distance);
                float priorityRank = distance.sqrMagnitude + angle * angle * 16f;     // Formula to choose closest enemy that's closest to player's field of view

                if (priorityRank < minPriorityRank && distance.magnitude <= player.detectionRange)
                {
                    closestTarget = potentialTarget;
                    minPriorityRank = priorityRank;
                    targetFound = true;
                }
            }

            // Deciding if player found a potential target
            if (targetFound)
            {
                // To remove any pre-existing locks
                if (player.isLockedOn)
                {
                    Destroy(targetLockClone.gameObject);
                }

                // Instantiating lock on effect
                var targetCollider = closestTarget.GetComponentInChildren<Collider>();
                player.target = closestTarget;
                targetLockClone = Instantiate(targetLock, targetCollider.transform.position, targetCollider.transform.localRotation, targetCollider.transform);
                targetLockClone.localScale = targetCollider.transform.localScale;
                targetLockClone.gameObject.layer = 8;   // 8 corresponds with TargetLock layer
                player.isLockedOn = true;
            }
        }
    }

    // Having the torso of player face the target if locked on (rotating torso also rotates attached weapons)
    void AdjustRotation()
    {
        // Retrieving torso with its parent's parent 
        Transform torso = transform.Find("Armature").Find("Torso");
        /*
            The below code can usually be done simply with LookAt() except because of Blender to Unity export issues the torso part has
            its y axis as its forward (but also backwards) so had to use Vector3.Angle() to find angle to swivel z axis to face target
            (z axis is up for torso bc of export issues)- the Angle() method also only does angles 0-180 degrees so InverseTransformPoint
            is used to store target's location as local to compare with player's position to see if it is left or right of player
        */

        var targetDir = new Vector3(player.target.transform.position.x - torso.transform.position.x,   // Vector3 representing direction between 
                            0f, player.target.transform.position.z - torso.transform.position.z).normalized;   // player and target

        float angle = Vector3.Angle(targetDir, player.transform.forward);   // Angle between two vectors

        Vector3 targetLocalPos = transform.InverseTransformPoint(player.target.position);  // Storing target's location relative to player

        if (targetLocalPos.x < 0)   // If target is left of player
            lookRotation = Quaternion.Slerp(lookRotation, Quaternion.Euler(-90f, 0f, -angle), 10 * Time.deltaTime);  // Not sure how putting lookRotation on right of equals sign works yet but it overrides animation without jitters
        else if (targetLocalPos.x > 0)  // If target is right of player
            lookRotation = Quaternion.Slerp(lookRotation, Quaternion.Euler(-90f, 0f, angle), 10 * Time.deltaTime);  // Not sure how putting lookRotation on right of equals sign works yet but it overrides animation without jitters

        torso.localRotation = lookRotation;
    }
}

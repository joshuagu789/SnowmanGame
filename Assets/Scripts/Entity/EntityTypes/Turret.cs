/*
 * A Turret script is give to any game object that is a robot but attached to another Entity
 *  - turrets (like robots) explode into pieces when its health reaches zero
 *  - turrets don't use nav mesh agents or follow leader instructions so it overrides some methods 
 *  - turrets do not add themselves to the game server so they cannot be targeted
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Turret : Robot
{
    public Entity owner;    // Entity to which the turret is attached to
    public bool focusFires; // Entity will target owner's target if bool is true
    public bool spotsTargets;   // Entity will tell owner to target enemy if owner has no target yet
    private Transform turretFront;  // For measuring the angle that the target is away from the turret's front (which is not necessarily the front of the gameObject's transform)

    private float vectorTimer = 1f;

    public override void OnEnable()
    {
        // Doesn't remove from server
        AimConstraint aimer = GetComponentInChildren<AimConstraint>();
        if (aimer != null)
            turretFront = aimer.transform;
        else
            turretFront = transform;
    }

    public override void OnDisable()
    {
        // Doesn't remove from server (was never in it in the first place)
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVectors();
        CheckDamage();
        UpdateStats();
        UpdateLockState();
        if (systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Like the usual UpdateLockState but also allows turret to lock onto owner's target
    public override void UpdateLockState()
    {
        if (owner.target == null)
        {
            target = null;
            isLockedOn = false;
            animator.SetBool("isLockedOn", false);
        }
        else if (target != null && owner.target != null && focusFires)
            FocusFire(owner.target);
        else if (target != null)
        {
            animator.SetBool("isLockedOn", true);
            isLockedOn = true;
        }
        else if (target == null && owner.target != null)
            FocusFire(owner.target);
    }

    // Entities usually ignore y axis (height) when calculating vectors but turrets need to override and use all 3 axes
    // since aiming needs to be more accurate for them
    public override void UpdateVectors()
    {
        if (isLockedOn && target != null)
        {
            vectorTimer += Time.deltaTime;
            if (vectorTimer >= 0.25f)
            {
                vectorTimer = 0;
                vectorToTarget = target.position - transform.position;
                //angleToTarget = Vector3.Angle(transform.forward, vectorToTarget);
                angleToTarget = Vector3.Angle(turretFront.forward, vectorToTarget);
                distanceToTargetSqr = vectorToTarget.sqrMagnitude;
            }
        }
    }

    public override void MoveTo(Vector3 location)
    {
        owner.MoveTo(location);
    }
}

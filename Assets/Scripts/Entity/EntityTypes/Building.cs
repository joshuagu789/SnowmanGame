using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Child of Robot and Entity script that manages the unique characteristics that building entities have
/// <br/>
/// Typical traits: Explode and drop scrap metal on death, spawn disabled and need repairs to become fully operational, they also require materials to be built,
/// buildings can technically still lock onto targets and command squads (maybe even move)
/// </summary>
public class Building : Robot
{

    public List<Item> buildCost;
    private bool underConstruction = true;
    private bool isTouchingGround = true;   // Buildings are instantiated halfway into the ground since Spawner spawns units with their center of mass touching ground

    private void Awake()
    {
        systemIntegrity = maxIntegrity / 10;
        SetDisableAll(true);
    }

    // Update is called once per frame
    void Update()
    {
        CheckConstructionState();
        UpdateVectors();
        CheckDamage();
        UpdateStats();
        UpdateLockState();
        if (systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void CheckConstructionState()
    {
        if (systemIntegrity >= maxIntegrity && underConstruction)
        {
            SetDisableAll(false);
            underConstruction = false;
        }
        // Checking to see if building has emerged out of ground yet
        var personalCollider = GetComponent<BoxCollider>();
        var colliders = Physics.OverlapCapsule(transform.position - new Vector3(0, personalCollider.size.y/2f, 0),
                        transform.position + new Vector3(0, personalCollider.size.y/2f, 0), personalCollider.size.x/4f);
        bool containsGround = false;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag.Equals("Ground"))
            {
                containsGround = true;
            }
        }
        if (!containsGround)
            isTouchingGround = false;
    }

    public override void Repair(float amount)
    {
        base.Repair(amount);
        if (isTouchingGround)   // Raising building up as it is being built
        {
            transform.position += new Vector3(0, GetComponent<BoxCollider>().size.y * amount / maxIntegrity / 2, 0);
        }
    }

    public override void UpdateLockState()
    {
        if (target == null || !isLockedOn)
        {
            isLockedOn = false;
            animator.SetBool("isLockedOn", false);
        }

        if (distanceToTargetSqr > detectionRange * detectionRange)
        {
            vectorToTarget = new Vector3(0f, 0f, 0f);
            distanceToTargetSqr = 0;
            isLockedOn = false;
            target = null;
        }
    }

    public override void MoveTo(Vector3 location)
    {
        // Does nothing for now
    }

    public override void StandStill()
    {
        // Does nothing for now
    }

    public override void Root()
    {
        // Does nothing for now
    }

    public override void Unroot()
    {
        // Does nothing for now
    }
}

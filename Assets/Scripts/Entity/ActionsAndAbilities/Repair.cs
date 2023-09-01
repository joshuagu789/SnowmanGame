/*
 * Allows entities equipped with this script to find and repair nearby ally robots when not in combat
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour
{
    [SerializeField]
    private Entity entity;
    [SerializeField]
    private float repairSpeed;
    [SerializeField]
    private float repairRange;
    [SerializeField]
    private float noticeRange;

    private Robot repairTarget;
    private float distanceToTargetSqr;
    private float timer;
    private float distanceTimer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!entity.isLockedOn && timer >= 1f && !entity.isDisabled && repairTarget == null)  // If entity not in combat
        {
            timer = 0;
            SearchRepairTarget();
        }
        else if (repairTarget != null)
            RepairRobot(repairTarget);
        else
        {
            repairTarget = null;
        }
    }

    // Searching for a wounded ally robot that's in range
    private void SearchRepairTarget()
    {
        var colliders = Physics.OverlapCapsule(transform.position + Vector3.down * 1000, transform.position + Vector3.up * 1000, noticeRange);
        foreach (Collider collider in colliders)
        {
            var robot = collider.gameObject.GetComponentInParent<Robot>();
            if (robot != null && robot.type.Equals(entity.type) && robot.systemIntegrity < robot.maxIntegrity)
            {
                repairTarget = robot;
                break;
            }
        }
    }

    // Moving to the chosen robot and repairing it when in range
    //  - NOTE: needs to be called continuously through update as of right now
    private void RepairRobot(Robot robot)
    {
        entity.isDisabled = true;
        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 0.25)
        {
            distanceTimer = 0;
            distanceToTargetSqr = new Vector3(robot.transform.position.x - entity.transform.position.x, 0f, robot.transform.position.z - entity.transform.position.z).sqrMagnitude;
        }

        if (distanceToTargetSqr <= repairRange * repairRange)   // If repair target is in range
        {
            entity.StandStill();
            entity.animator.SetBool("isBuilding", true);
            robot.Repair(repairSpeed * Time.deltaTime);
        }
        else
        {
            entity.MoveTo(robot.transform.position);
            entity.animator.SetBool("isBuilding", false);
        }
        
        if (robot.systemIntegrity >= robot.maxIntegrity || entity.isLockedOn)
        {
            repairTarget = null;
            entity.animator.SetBool("isBuilding", false);
            entity.isDisabled = false;
        }
    }
}

/*
 * Increases game server's detection level as long as the entity holding this meets certain requirements 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public Entity entity;

    public float range;
    public float energyCost;
    public float detectionSpeed;

    // Update is called once per frame
    void Update()
    {
        if (entity.isLockedOn && transform != null && !entity.isDisabled && entity.energy > 0 && entity.distanceToTargetSqr <= range * range)
        {
            SpotTarget();
        }
    }

    private void SpotTarget()
    {
        entity.energy -= energyCost * Time.deltaTime;
        entity.server.RaiseDetectionLevel(detectionSpeed * Time.deltaTime);
    }
}

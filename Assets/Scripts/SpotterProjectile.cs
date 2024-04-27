/*
 * Component added on to projectile game objects that allow it to spot nearby enemies as it flies and relay the information back to the 
 * HuntingProjectileAttack script
 *  - component also can be used with explosive bullet script
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotterProjectile : MonoBehaviour
{
    private float spotRadius;
    private EntityType ownerType;
    private HuntingProjectileAttack projectileOwner;

    private bool hasSentInformation;    // SpotterProjectile can only relay information once- when this bool is set to false
    private float timer;

    public void SetStats(float spotRadius, EntityType ownerType, HuntingProjectileAttack projectileOwner)
    {
        this.spotRadius = spotRadius;
        this.ownerType = ownerType;
        this.projectileOwner = projectileOwner;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 0.25f && !hasSentInformation)
        {
            timer = 0;
            SpotEnemies();
        }
    }

    private void SpotEnemies()
    {
        // Putting all colliders that could belong to enemies inside this list (height of overlap capsule is high since only radius used to measure distance)
        var colliders = Physics.OverlapCapsule(transform.position + Vector3.down * 1000, transform.position + Vector3.up * 1000, spotRadius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponentInParent<Entity>() != null && !collider.gameObject.GetComponentInParent<Entity>().GetEntityType().Equals(ownerType))
            {
                projectileOwner.ReportTarget(collider.transform.position);
                hasSentInformation = true;
                break;
            }
        }
    }
}

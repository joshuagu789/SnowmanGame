/*
 * Entities with this script will seek out nearby Harvestables and extract resources from them when out of combat
 *  - Extremely similar to Repair in structure
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class Harvester : MonoBehaviour
{
    [SerializeField]
    private Entity entity;
    [SerializeField]
    private float harvestSpeed;
    [SerializeField]
    private float harvestRange;
    [SerializeField]
    private float noticeRange;
    [SerializeField]
    private float outputAmount; // Units of the resource assigned to the extracted item
    private float currentAmount;    // Current units of the resource the harvester has stored- will create an extracted item once currentAmount exceeds outputAmount
    [SerializeField]
    private Transform outputLocation;   // Where the items taken from the Harvestable containing the resource are instantiated

    private Harvestable harvestTarget;
    private GameObject resourcePrefab;
    private float distanceToTargetSqr;
    private float timer;
    private float distanceTimer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (!entity.isLockedOn && timer >= 1f && !entity.isDisabled && harvestTarget == null)  // If entity not in combat
        {
            timer = 0;
            SearchHarvesTarget();
        }
        else if(resourcePrefab != null)
            Harvest(harvestTarget);
    }

    // Searching for a game object with script harvestable that's within range
    private void SearchHarvesTarget()
    {
        var colliders = Physics.OverlapCapsule(transform.position + Vector3.down * 1000, transform.position + Vector3.up * 1000, noticeRange);
        foreach (Collider collider in colliders)
        {
            var resource = collider.gameObject.GetComponentInParent<Harvestable>();
            if (resource != null)
            {
                harvestTarget = resource;
                resourcePrefab = resource.GetResource().prefab;
                currentAmount = 0;
                break;
            }
        }
    }

    // Moving to the chosen Harvestable and mining it when in range
    //  - NOTE: needs to be called continuously through update as of right now
    private void Harvest(Harvestable resource)
    {
        entity.isDisabled = true;

        distanceTimer += Time.deltaTime;

        if (distanceTimer >= 0.25 && resource != null)
        {
            distanceTimer = 0;
            distanceToTargetSqr = new Vector3(resource.transform.position.x - entity.transform.position.x, 0f, resource.transform.position.z - entity.transform.position.z).sqrMagnitude;
        }

        if (distanceToTargetSqr <= harvestRange * harvestRange && resource != null)   // If repair target is in range
        {
            entity.StandStill();
            entity.animator.SetBool("isBuilding", true);
            currentAmount += harvestSpeed * Time.deltaTime;
            resource.Mine(harvestSpeed * Time.deltaTime);

            var targetRotation = Quaternion.LookRotation(new Vector3(resource.transform.position.x - transform.position.x, 0f, resource.transform.position.z - transform.position.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, entity.rotationSpeed / 180 * Time.deltaTime);

            // Spawning the game object as an item holding the resource and its amount
            if (currentAmount >= outputAmount)
            {
                currentAmount = 0;
                var item = Instantiate(resourcePrefab, outputLocation.position, transform.rotation);
                item.GetComponent<Item>().SetAmount((int)outputAmount);
            }
        }
        else if(resource != null)
        {
            entity.MoveTo(resource.transform.position);
            entity.animator.SetBool("isBuilding", false);
        }
        
        if (entity.isLockedOn || resource == null) 
        {
            // Spawning the game object holding the resource and its amount early without reaching the desired outputAmount since harvester was interrupted
            if (currentAmount > 0)
            {
                var item = Instantiate(resourcePrefab, outputLocation.position, transform.rotation);
                item.GetComponent<Item>().SetAmount((int)currentAmount);
            }

            harvestTarget = null;
            resourcePrefab = null;
            entity.animator.SetBool("isBuilding", false);
            entity.isDisabled = false;
        }
    }
}

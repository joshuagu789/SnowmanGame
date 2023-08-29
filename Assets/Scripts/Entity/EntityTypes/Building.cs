/*
 * Buildings, like robots, drop scrap metal and explode on death, and they also spawn disabled and need repairs to become fully operational
 *  - they also require materials to be built
 *  - buildings can technically still lock onto targets and command squads (maybe even move)
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Robot
{
    public List<Item> buildCost;
    private bool underConstruction = true;
    private bool isTouchingGround = true;   // Buildings are instantiated halfway into the ground since Spawner spawns units with their center of mass touching ground

    private void Start()
    {
        systemIntegrity = maxIntegrity / 10;
        isDisabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
            Repair(15 * Time.deltaTime);
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
            isDisabled = false;
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

    public override void MoveTo(Vector3 location)
    {
        FaceLocation(location);
    }

    public override void StandStill()
    {
        // Does nothing
    }
}

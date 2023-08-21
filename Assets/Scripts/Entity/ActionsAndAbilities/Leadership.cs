/*
 * Script that's usually attached to leaders to allow them to coordinate their team
 *  - can move entire squad's location if one finds an enemy
 *  - can recruit nearby allies if leader has room in squad
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leadership : MonoBehaviour
{
    public Entity entity;
    public float maxFollowers;
    public float recruitRange;

    private float recruitmentTimer;
    private bool hasBroadcastedDestination;

    // Update is called once per frame
    void Update()
    {
        recruitmentTimer += Time.deltaTime;
        if (recruitmentTimer >= 5f && entity.squadList != null && entity.squadList.Count < maxFollowers)    // If leader has more room for squad members
        {
            recruitmentTimer = 0;
            RecruitNearby();
        }
        if (entity.isLockedOn && entity.target != null && !hasBroadcastedDestination)
        {
            HuntTarget();
            hasBroadcastedDestination = true;
        }
        else if (!entity.isLockedOn && !hasBroadcastedDestination)
            hasBroadcastedDestination = false;  // Resetting bool for it to be used again when leader finds new target
    }

    // Leader gathers all nearby colliders into a list then searches through them for potential allies to add to squad
    private void RecruitNearby()
    {
        var colliderList = Physics.OverlapSphere(transform.position, recruitRange);
        foreach (Collider collider in colliderList)
        {
            var colliderEntity = collider.gameObject.GetComponentInParent<Entity>();

            // If collider belongs to an ally with no leader and it is able to join a squad
            if (colliderEntity != null && colliderEntity.canJoinSquad && colliderEntity.leader == null
                && colliderEntity.type.Equals(gameObject.GetComponent<Entity>().type) && entity.squadList.Count < maxFollowers)
            {
                colliderEntity.leader = transform;
                entity.squadList.Add(colliderEntity);
            }
        }
    }

    // All allies not in combat will move to the entity that the leader is targeting
    private void HuntTarget()
    {
        foreach (Entity ally in entity.squadList)
        {
            var location = new Vector3(entity.target.position.x + Random.Range(-50f, 50f), entity.target.position.y, entity.target.position.z + Random.Range(-50f, 50f));
            if (!ally.isLockedOn)
                ally.MoveTo(location);
        }
    }
}

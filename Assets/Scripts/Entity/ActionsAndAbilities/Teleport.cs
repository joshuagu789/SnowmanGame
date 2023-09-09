using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/*
 * Script intended for player to teleport team across large distances by using ModelD's teleport ability
 *  - could possibly make entities use them independently without player's command
 *  - Planned: leader AIs can command entities to teleport squad by calling on public method?
 */

public class Teleport : SquadAbility
{
    public GameObject teleportVFX;
    public Transform effectLocation;    // Where teleportVFX is created

    public float teleportDistance;
    public float activationDelay;
    public float radius;    // Also teleports allies within radius

    private void Awake()
    {
        type = AbilityType.Teleport;
    }

    public override void UseAbility(Vector3 direction)
    {
        if (CanUseAbility())
        {
            ResetCooldown();
            ExpendEnergy();

            //var travelVector = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z) * teleportDistance;
            var travelVector = direction * teleportDistance;
            StartCoroutine(MovePosition(entity.leader.GetComponent<Entity>().squadList, travelVector, entity.leader.GetComponent<Entity>()));
        }
    }

    public override void UseAbility(Entity target)
    {
        // Planned: teleport squad to target if in range
    }

    public override string GetAbilityType() { return "Teleport"; }

    // Either moves everyone inside list (which includes itself) or just moves itself
    private IEnumerator MovePosition(List<Entity> list, Vector3 travelVector, Entity leader)
    {
        entity.animator.SetBool("isMoving", false);
        entity.animator.SetTrigger("Teleport");
        entity.isDisabled = true;
        entity.agent.isStopped = true;

        yield return new WaitForSeconds(activationDelay);

        if (list != null)
        {
            foreach (Entity ally in list)
            {
                Instantiate(teleportVFX, ally.transform.position, transform.rotation);
                
                RaycastHit hit;
                // If destination is below/above the ground
                if (Physics.Raycast(ally.transform.position + travelVector + transform.up * -1000, transform.up, out hit, Mathf.Infinity) || Physics.Raycast(ally.transform.position + travelVector + transform.up * 1000, -transform.up, out hit, Mathf.Infinity))
                    if (hit.collider.gameObject.tag.Equals("Ground"))
                        ally.agent.Warp(hit.point);

                ally.agent.enabled = true;
                ally.isIdle = false;
            }
            
            // Moving the leader outside of the loop for moving the squad since leader could be the player and the player is moved in a different manner than AI's using NavMeshAgents
            if (leader.gameObject.GetComponent<Player>() != null)
            {
                yield return new WaitForSeconds(0.1f);
                var player = entity.leader.GetComponent<CharacterController>(); // Player moves using character controller
                Instantiate(teleportVFX, player.transform.position, transform.rotation);
                player.Move(new Vector3(0f, 100f, 0f));     // To clear player of any obstacles NOTE: might cause problems if game gets a ceiling in future
                player.Move(travelVector); 
                player.Move(new Vector3(0f, -200f, 0f));    // Setting player back down
                Instantiate(teleportVFX, player.transform.position, transform.rotation);
            }
            else
                entity.leader.gameObject.transform.Translate(travelVector);            
        }

        entity.agent.isStopped = false;
        entity.isDisabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/*
 * Script intended for player to teleport team across large distances by using ModelD's teleport ability
 *  - could possibly make entities use them independently without player's command
 *  - Planned: leader AIs can command entities to teleport squad by calling on public method?
 */

public class Teleport : MonoBehaviour
{
    public Entity entity;
    public Camera camera;
    public GameObject teleportVFX;
    public Transform effectLocation;    // Where teleportVFX is created

    public float teleportDistance;
    public float energyCost;
    public float cooldown;
    public float activationDelay;
    public float radius;    // Also teleports allies within radius

    private float cooldownTimer = 0;
    private Vector3 destination;

    // Update is called once per frame
    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (entity.leader.gameObject.CompareTag("Player") & Input.GetKeyDown(KeyCode.F) && cooldownTimer >= cooldown && entity.energy >= energyCost)
        {
            cooldownTimer = 0;
            entity.energy -= energyCost;

            var travelVector = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z) * teleportDistance;

            StartCoroutine(MovePosition(entity.leader.GetComponent<Entity>().squadList, travelVector, true));
        }
        // Below two lines are experimental (for entities teleporting indepdendently)
        else if (entity.leader != null && cooldownTimer >= cooldown) { }
        else { }
    }

    // Either moves everyone inside list (which includes itself) or just moves itself
    private IEnumerator MovePosition(List<Entity> list, Vector3 travelVector, bool leaderIsPlayer)
    {
        entity.animator.SetBool("isMoving", false);
        entity.animator.SetTrigger("Teleport");
        entity.isDisabled = true;
        entity.agent.isStopped = true;
        print(travelVector);
        yield return new WaitForSeconds(activationDelay);

        if (list != null)
        {
            //list.Add(entity.leader.GetComponent<Entity>());
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
            
            // Moving the leader of the squad since list is leader's squad list & doesn't include itself
            if (leaderIsPlayer)
            {
                yield return new WaitForSeconds(0.1f);
                var player = entity.leader.GetComponent<CharacterController>(); // Player moves using character controller
                Instantiate(teleportVFX, player.transform.position, transform.rotation);
                player.Move(new Vector3(0f, 100f, 0f));     // To clear player of any obstacles
                player.Move(travelVector); 
                player.Move(new Vector3(0f, -200f, 0f));    // Setting player back down
                Instantiate(teleportVFX, player.transform.position, transform.rotation);
            }
            else
                entity.leader.gameObject.transform.Translate(travelVector);
            
            //Instantiate(teleportVFX, effectLocation.position, transform.rotation);
        }

        entity.agent.isStopped = false;
        entity.isDisabled = false;
    }
}

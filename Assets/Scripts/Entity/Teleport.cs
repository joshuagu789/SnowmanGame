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

            destination = new Vector3(camera.transform.forward.x, 0f, camera.transform.forward.z);
            destination *= teleportDistance;
            var travelVector = destination - new Vector3(entity.leader.transform.position.x, 0f, entity.leader.transform.position.z);

            StartCoroutine(MovePosition(entity.leader.GetComponent<Entity>().squadList, travelVector, true));
        }
        // Below two lines are experimental (for entities teleporting indepdendently)
        else if (entity.leader != null && cooldownTimer >= cooldown) { }
        else { }
    }

    // Either moves everyone inside list (which includes itself) or just moves itself
    private IEnumerator MovePosition(List<Transform> list, Vector3 travelVector, bool leaderIsPlayer)
    {
        entity.animator.SetBool("isMoving", false);
        entity.animator.SetTrigger("Teleport");
        entity.isDisabled = true;
        entity.agent.isStopped = true;

        yield return new WaitForSeconds(activationDelay);
        Instantiate(teleportVFX, effectLocation.position, transform.rotation);

        if (list != null)
        {
            foreach (Transform ally in list)
            {
                ally.transform.position += travelVector;
                ally.GetComponent<Entity>().isIdle = false; // Find some way to clear walk points of entities
            }

            // Moving the leader of the squad since list is leader's squad list & doesn't include itself
            if (leaderIsPlayer)
            {
                yield return new WaitForSeconds(0.1f);
                entity.leader.GetComponent<CharacterController>().Move(travelVector); // Player moves using character controller 
            }
            else
                entity.leader.gameObject.transform.Translate(travelVector);
            Instantiate(teleportVFX, effectLocation.position, transform.rotation);
        }

        entity.agent.isStopped = false;
        entity.isDisabled = false;
    }
}

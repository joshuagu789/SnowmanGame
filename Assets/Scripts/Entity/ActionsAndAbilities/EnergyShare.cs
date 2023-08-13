/*
 * Entity equipped with this script will search through all allies close enough to it, check if their energy is low, and sacrifice some of its 
 * own energy to supply its ally with it
 *  - Planned: have entity take energy from allies if entity's energy is too low and allies have abundant energy
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnergyShare : MonoBehaviour
{
    public Entity entity;
    public Transform transferOrigin;

    public GameObject tetherBeam;   // Using an empty gameObject with line renderer instead of just line renderer component since need to instantiate multiple
    public float range;
    public float transferSpeed;
    public int maxTethers;

    // Float key stores instance ID of energy share targets which unlocks the position (Transform) and visual energy beam (LineRenderer) of target
    private Dictionary<int, List<Object>> activeTethers = new Dictionary<int, List<Object>>();
    private float timer = 1;

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0;
            UpdateTargets();
        }
        TransferEnergy();
    }

    // Adding new candidates (allies w/ low energy) to activeTargets and clearing targets that are too far away
    private void UpdateTargets()
    {
        var allColliders = Physics.OverlapSphere(transferOrigin.position, range);   
        foreach (Collider potentialTarget in allColliders)
        {
            var character = potentialTarget.transform.gameObject.GetComponentInParent<Entity>();

            // Checking if target is an ally/itself, if target isn't already on the list for energy share, and if target has a lower energy ratio
            if (character != null && entity.energy > 0 && character.type.Equals(entity.type) && character.gameObject.GetInstanceID()
                != transform.gameObject.GetInstanceID() && !activeTethers.ContainsKey(character.gameObject.GetInstanceID())
                && character.energy / character.maxEnergy < entity.energy / entity.maxEnergy && activeTethers.Count < maxTethers)
            {
                var beam = Instantiate(tetherBeam);
                var transferTarget = new List<Object> { character.transform, beam };
                activeTethers.Add(character.gameObject.GetInstanceID(), transferTarget);
            }
        }
    }

    // Transfer energy over to all targets that have a lower energy ratio and are close enough in Dictionary activeTargets 
    private void TransferEnergy()
    {
        foreach (int key in activeTethers.Keys.ToListPooled())  // Need to use ToListPooled() since foreach loop will stop when dictionary gets modified otherwise
        {
                            // character.transform
            var character = activeTethers[key][0].GetComponentInChildren<Entity>();

                       // Gameobject tetherBeam
            var beam = activeTethers[key][1].GetComponent<LineRenderer>();

            var distanceToTarget = new Vector3(character.transform.position.x - entity.transform.position.x, 0f, character.transform.position.z -
                                    entity.transform.position.z);

            if (character.energy / character.maxEnergy < entity.energy / entity.maxEnergy && entity.energy > 0 &&
                distanceToTarget.sqrMagnitude <= (range * range)) // If target is outside range (square magnitude compared to range squared is same as magnitude compared to range but more efficient)
            {
                character.energy += transferSpeed * Time.deltaTime;     // Transfer energy
                entity.energy -= transferSpeed * Time.deltaTime;

                beam.SetPosition(0, transferOrigin.position);       // Update line renderer position
                beam.SetPosition(1, character.transform.position);
                beam.enabled = true;
            }
            else 
            {
                Destroy(activeTethers[key][1]);                  // Removing from list
                activeTethers.Remove(key);
            }
        }
    }


}

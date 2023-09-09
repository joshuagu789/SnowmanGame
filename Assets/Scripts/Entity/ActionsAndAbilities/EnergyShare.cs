/*
 * Entity equipped with this script will search through all allies close enough to it, check if their energy is low, and sacrifice some of its 
 * own energy to supply its ally with it
 *  - EnergyShare is also a SquadAbility with the UseAbility (Entity target) method making the entity cancel its own independent energy tethers to 
 *    focus all of its energy transfer onto the specified target
 *  - NOTE: currently tracks energy targets and visible beams attached to them through Dictionary<int, List<Object>> where int is target's unique ID
 *          and List<Object> contains target's transform and attached energy beam- BUT maybe ID not needed since target's transform already unique??
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnergyShare : SquadAbility
{
    public Transform transferOrigin;

    public GameObject tetherBeam;   // Using an empty gameObject with line renderer instead of just line renderer component since need to instantiate multiple
    public float range;
    public float transferSpeed;
    public int maxTethers;

    // Float key stores instance ID of energy share targets which unlocks the position (Transform) and visual energy beam (LineRenderer) of target
    private Dictionary<int, List<Object>> activeTethers = new Dictionary<int, List<Object>>();
    private float timer = 1;

    private void Awake() { type = AbilityType.FocusEnergy; }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0;
            UpdateTargets();
        }
        if(activeTethers.Count > 0)
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

    // Find nearest ally based on proximity to Vector3 direction
    public override void UseAbility(Vector3 direction)
    {
        ResetCooldown();
        ExpendEnergy();
        // Getting all nearby allies in same squad
        List<Entity> allyList = new List<Entity>();
        if (entity.leader != null)
        {
            var leader = entity.leader.gameObject.GetComponent<Entity>();
            allyList = new List<Entity>(leader.squadList);
            allyList.Add(leader);
        }

        allyList.Remove(entity);    // Removing self

        if (allyList.Count > 0)
        {
            bool targetFound = false;
            Entity closestTarget = null;
            float minPriorityRank = Mathf.Infinity;

            // Finding ally closest and nearest to direction's position and field of view respectively 
            foreach (Entity ally in allyList)
            {
                Vector3 distance = new Vector3(ally.transform.position.x - transform.position.x, 0f, ally.transform.position.z - transform.position.z);
                float angle = Vector3.Angle(direction, distance);
                float priorityRank = distance.sqrMagnitude + angle * angle * 16f;    // Formula to choose closest enemy that's closest to player's field of view
                                                                                                            // If ally is not self
                if (priorityRank < minPriorityRank && distance.sqrMagnitude <= range * range && ally.GetInstanceID() != gameObject.GetInstanceID() && ally.energy < ally.maxEnergy) 
                {
                    closestTarget = ally;
                    minPriorityRank = priorityRank;
                    targetFound = true;
                }
            }

            if(targetFound == true)
                StartCoroutine(FocusEnergy(closestTarget));
        }
    }


    public override void UseAbility(Entity target)
    {
        var distance = new Vector3(target.transform.position.x - transform.position.x, 0f, target.transform.position.z - transform.position.z);
        if(distance.sqrMagnitude <= range * range * maxTethers)     // FocusEnergy also extends range by a bit
        StartCoroutine(FocusEnergy(target));
    }

    public override string GetAbilityType() { return "Focus Energy"; }

    // Focusing all tethers onto one target instead of spreading them out- stops once target gets enough energy or entity runs out of energy
    private IEnumerator FocusEnergy(Entity target)
    {
        // Cancelling all current energy transfers
        if (activeTethers.Count > 0)
            foreach (int key in activeTethers.Keys.ToListPooled())
                Destroy(activeTethers[key][1]);     // Destroys game objects for the visible beams
        activeTethers.Clear();

        var originalRange = range;
        range = 0f;     // So that entity with EnergyShare would not find new targets when focusing energy on specified target
        var originalTransferSpeed = transferSpeed;
        transferSpeed = transferSpeed * maxTethers;     // Also strengthening transferSpeed for the target

        var beam = Instantiate(tetherBeam);
        var tether = beam.GetComponent<LineRenderer>();

        // Transfering Energy
        while (entity.energy > 0 && target.energy < target.maxEnergy)
        {
            target.energy += transferSpeed * Time.deltaTime;     
            entity.energy -= transferSpeed * Time.deltaTime;

            tether.SetPosition(0, transferOrigin.position);       // Update line renderer position
            tether.SetPosition(1, target.transform.position);
            tether.enabled = true;
            yield return 0.01f;      // Coroutine only continues after condition is false
        }

        Destroy(beam);
        range = originalRange;
        transferSpeed = originalTransferSpeed;
    }
}

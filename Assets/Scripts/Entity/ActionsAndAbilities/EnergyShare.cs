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

    public override void UseAbility(Vector3 direction)
    {
        // Find nearest ally based on where player is looking where direction is new Vector3(camera.position,forward.x,0f,camera.position,forward.z)?
    }

    public override void UseAbility(Entity target)
    {
        StartCoroutine(FocusEnergy(target));
    }

    public override string GetAbilityType()
    {
        return "Energy Share";
    }

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
        var transferTarget = new List<Object> { target.transform, beam };
        activeTethers.Add(target.gameObject.GetInstanceID(), transferTarget);   // Adding target to activeTethers so TransferEnergy() will only work on that one particular target

        while (entity.energy > 0 && target.energy < target.maxEnergy)   
            yield return null;      // Coroutine only continues after condition is false 

        // Restoring EnergyShare to default state 
        foreach (int key in activeTethers.Keys.ToListPooled())  // Clearing all tethers again
            Destroy(activeTethers[key][1]);
        activeTethers.Clear();

        range = originalRange;
        transferSpeed = originalTransferSpeed;
    }
}

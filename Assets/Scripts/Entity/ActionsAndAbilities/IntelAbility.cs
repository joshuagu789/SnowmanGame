/*
 * Categorizes a group of scripts used specifically by allies of player to relay information to player (such as where enemies, resources, game objectives are)
 *  - could possibly have AI entities use these scripts to communicate with their AI leaders
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntelAbility : MonoBehaviour
{
    [SerializeField]
    private Entity entity;
    [SerializeField]
    private float inaccuracy;
    [SerializeField]
    private float range;
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private float cooldown;
    [SerializeField]
    private string scriptName;  // Specifically for player to read what ally's ability does
    [SerializeField]
    private string specialization;   // The name of the script that entity specifically searches for
    [SerializeField]
    private Compass compass;    // Using Compass script to get direction of specialization sources

    private float cooldownTimer;

    void Update()
    {
        Dictionary<GameObject, float> finish = new Dictionary<GameObject, float>();

        cooldownTimer += Time.deltaTime;
    }

    // Gathering all game objects with the Monobehaviour specialization and their directions from origin and returning it to the requester
    public Dictionary<GameObject, float> GetLocations(Transform origin)
    {
        Dictionary<GameObject, float> locations = new Dictionary<GameObject, float>();

        if (CanUseAbility())
        {
            ExpendEnergy();
            ResetCooldown();

            // Getting all colliders (usually game objects with colliders also have MonoBehaviours)
            var colliders = Physics.OverlapCapsule(transform.position + new Vector3(0, -100f, 0), transform.position + new Vector3(0, 100f, 0), range);
            foreach (Collider collider in colliders)
            {
                Entity entityTarget = collider.gameObject.GetComponentInParent<Entity>();
                var scripts = collider.gameObject.GetComponentsInParent<MonoBehaviour>();   // Getting all the scripts of collider's game object

                foreach (MonoBehaviour script in scripts)
                {
                    // If the specialization is Entity, also checks if target entity is a different type than the searcher entity (aka enemies) 
                    if (specialization.Equals("Entity") && entityTarget != null && !entityTarget.GetEntityType().Equals(entity.GetEntityType()) && !locations.ContainsKey(script.gameObject))
                    {
                        locations.Add(script.gameObject, (int)compass.GetDirection(script.transform, origin)); // Adding game object's info to dictionary if one of game object's scripts has the specialization
                        break;
                    }
                    else if (specialization.Equals(script.GetType().Name) && !locations.ContainsKey(script.gameObject))
                    {
                        locations.Add(script.gameObject, (int)compass.GetDirection(script.transform, origin)); // Adding game object's info to dictionary if one of game object's scripts has the specialization
                        break;
                    }
                }
            }
        }
        return locations;
    }

    public bool CanUseAbility() { return IsOffCooldown() && HasEnoughEnergy() && !entity.isDisabled; }
    public bool IsOffCooldown() { return cooldownTimer >= cooldown; }
    public bool HasEnoughEnergy() { return entity.energy >= energyCost && entity.energy != 0; }
    public void ResetCooldown() { cooldownTimer = 0; }
    public void ExpendEnergy() { entity.energy -= energyCost; }
    public string GetAbilityType() { return scriptName; }
}


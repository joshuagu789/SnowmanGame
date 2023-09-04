/*
 * Script mainly intended for Model B to not target or attack enemies so that it can conserve energy as well as not alert enemies to group's presence
 *  - Done by disabling attacks of Model B along with its turrets
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HoldFire : SquadAbility
{
    [SerializeField]
    private float duration;

    // All attacks of entity and an int for its original cooldown (cooldown will be set to infinity while weapons are locked)
    private Dictionary<Attack, float> attacksList = new Dictionary<Attack, float>();

    public override void UseAbility(Vector3 direction) { StartCoroutine(LockWeapons()); }
    public override void UseAbility(Entity target) { StartCoroutine(LockWeapons()); }
    public override string GetAbilityType() { return "Hold Fire"; }

    // Gets all entities and their corresponding Attacks (HoldFire owner could have Turrets as children with their own Attacks) and disabling them for a duration
    private IEnumerator LockWeapons()
    {
        Entity[] attackers = GetComponentsInChildren<Entity>();

        if (attackers != null)
        {
            // Storing all entities that can attack and their attack scripts in dictionary 
            foreach (Entity attacker in attackers)
            {
                Attack[] attacksArray = attacker.gameObject.GetComponents<Attack>();
                if (attacksArray != null)
                {
                    //List<Attack> attacks = new List<Attack>();  // Need to convert from array to list
                    //attacks.AddRange(attacks);
                    foreach (Attack attack in attacksArray)
                    {
                        var originalCooldown = attack.GetCooldown();
                        attack.SetCooldown(Mathf.Infinity);             // Essentially disables attack
                        attacksList.Add(attack, originalCooldown);
                    }
                }
            }
        }

        yield return new WaitForSeconds(duration);

        foreach (Attack attack in attacksList.Keys) { attack.SetCooldown(attacksList[attack]); }    // Restoring original cooldowns
        attacksList.Clear();
    }
}

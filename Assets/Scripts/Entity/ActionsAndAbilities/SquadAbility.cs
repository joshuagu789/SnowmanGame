/*
 * Script to categorize certain abilities that some entities can use whether if they do it independently or commanded to by leader (leader can be player)
 *  - First use of abstract classes in project rather than inheritance 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
    Teleport,
    Build,
    HoldFire,
    //Radar,      // Especially meant for player's Model B ally to sense nearby enemies
    FocusEnergy,    // Focusing who to give energy to
    SenseObjectives     // Especially meant for player's Model C ally to sense game objective, Model B to sense enemies, Model D to sense resources
}

public abstract class SquadAbility : MonoBehaviour
{
    public Entity entity;
    public AbilityType type;
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private float cooldown;
    private float cooldownTimer;

    public virtual void Update() { cooldownTimer += Time.deltaTime; }

    public abstract void UseAbility( Vector3 direction );
    public abstract void UseAbility( Entity target );
    public abstract string GetAbilityType();

    public bool CanUseAbility() { return IsOffCooldown() && HasEnoughEnergy() && !entity.isDisabled; }
    public bool IsOffCooldown() { return cooldownTimer >= cooldown; }
    public bool HasEnoughEnergy() { return entity.energy >= energyCost && entity.energy != 0; }
    public void ResetCooldown() { cooldownTimer = 0; }
    public void ExpendEnergy() { entity.energy -= energyCost; }
}

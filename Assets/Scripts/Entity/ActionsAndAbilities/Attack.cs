/*
 * Abstract class Attack allows every type of attack (ranged, melee, energy transfer) to be referred to by the word "Attack"
 *  - Every Attack needs to override abstract method AttackTarget
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    [SerializeField]
    private float cooldown;

    public float GetCooldown() { return cooldown; }
    public void SetCooldown(float amount) { cooldown = amount; }
}

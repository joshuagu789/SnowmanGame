using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script that entities use to detect damage
 *  - reason this isn't inside the Entity script is because colliders for damage detection may not
 *    always be in parent gameobject (where Entity script is) and might be inside armature instead
 *  - Register scripts usually attached to same game object that damage colliders are attached to
*/

public class Register : MonoBehaviour
{
    private Transform damageSource;
    private float damageTaken;
    private float tempIncrease;
    private bool hasTakenDamage = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasTakenDamage)
        {
            damageSource = null;
            damageTaken = 0;
            tempIncrease = 0;
        }
    }

    public void TakeDamage(Transform damageSource, float damageTaken, float tempModifier)
    {
        this.damageSource = damageSource;
        this.damageTaken = damageTaken;
        this.tempIncrease = tempModifier;

        // Now announces to scripts checking DamageRegister that the game object has taken damage and needs to register it
        // hasTakenDamage will be set to false after the damage has been registered
        hasTakenDamage = true;
    }

    public bool HasTakenDamage() { return hasTakenDamage; }
    public Transform GetDamageSource() { return damageSource; }
    public float GetDamageTaken() { return damageTaken; }
    public float GetTempIncrease() { return tempIncrease; }
    public void ResetRegister() { hasTakenDamage = false; }
}

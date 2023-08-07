using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * First script created to encompass all smaller player scripts as well as holding vital player info
 *  - Later replace this with the Entity script?
 */

public class Player : Snowman
{
    private bool isRepairing = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        AddToServer();
    }

    private void OnDisable()
    {
        RemoveFromServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLockedOn & target != null)
        {
            UpdateVectors();
        }

        UpdateStats();
        CheckMelt();
        if (Input.GetKeyDown(KeyCode.R))
            isRepairing = !isRepairing;
        if(isRepairing)
            RepairDamage();
    }

    void UpdateStats()
    {
        // Calculating how to change integrity (aka health) based on temperature 
        if (temperature > minTemperature)
            systemIntegrity -= temperature / 10f * Time.deltaTime;

        if (register.hasTakenDamage)
        {
            systemIntegrity -= register.damageTaken;
            temperature += register.tempModifier;
            register.hasTakenDamage = false;
        }

        ClampStats();
    }
}

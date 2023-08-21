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

    private void OnEnable()
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
        agent.ResetPath();  // Player has a NavMeshAgent so that their allies will stop bumping into them- 
                            // constantly resetting path since agent is not used for travel but rather WASD controls
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

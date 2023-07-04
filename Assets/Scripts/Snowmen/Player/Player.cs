using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Snowman
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // Starting stats of the player
        systemIntegrity = 100f;
        maxIntegrity = 100f;

        temperature = 0f;
        minTemperature = 0f;

        integrityRegen = 1f;
        integrityLoss = 0f;
        tempGain = 0f;
        tempLoss = 1f;

        speed = 6f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckMelt();
    }

    
    public void CheckMelt()
    {
        // If player is going to melt
        if (systemIntegrity <= 50f)
        {
            animator.SetBool("isMelting", true);
            speed = 1f;
            print("melting");
        }
        else
        {
            animator.SetBool("isMelting", false);
            speed = 6f;
        }
    }
    
}

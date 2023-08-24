/*
 * Script used to help turret-like game objects face their targets by modifying their aim constraint component
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TurretAimer : MonoBehaviour
{
    public Entity entity;
    public AimConstraint aimer;
    private ConstraintSource aimSource;

    // Start is called before the first frame update
    void Start()
    {
        //aimer.enabled = false;
        aimer.constraintActive = false;
        aimSource = aimer.GetSource(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.isLockedOn && entity.target != null)
        {
            //aimer.enabled = true;
            aimer.constraintActive = true;
            aimSource.sourceTransform.position = entity.target.position;
        }
        else
            aimer.constraintActive = false;
        //aimer.enabled = false;
    }
}

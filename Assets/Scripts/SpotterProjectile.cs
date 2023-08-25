/*
 * Component added on to projectile game objects that allow it to spot nearby enemies as it flies and relay the information back to the 
 * HuntingProjectileAttack script
 *  - component also can be used with explosive bullet script
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotterProjectile : MonoBehaviour
{
    private float spotRadius;
    private HuntingProjectileAttack projectileOwner;

    public void SetStats(float spotRadius, HuntingProjectileAttack projectileOwner)
    {
        this.spotRadius = spotRadius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

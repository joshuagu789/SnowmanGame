/*
 * Script added to projectiles to make them spin in a random direction
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningProjectileEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var projectile = gameObject.GetComponent<Rigidbody>();
        projectile.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

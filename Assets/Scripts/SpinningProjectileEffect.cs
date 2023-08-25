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
        StartCoroutine(ApplyTorque());
    }

    private IEnumerator ApplyTorque()
    {
        yield return new WaitForSeconds(0.05f);     // Brief delay so that torque won't interfere with projectile's trajectory
        var projectile = gameObject.GetComponent<Rigidbody>();
        projectile.AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized, ForceMode.Impulse);
    }
}

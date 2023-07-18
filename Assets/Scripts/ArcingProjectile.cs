using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Instantiated by script ArcingProjectileAttack.cs- applies AOE damage when hitting a collider and instantiates an explosion script 
 *  - explosion script just for visual effects- damage is calculated in this script
 */

public class ArcingProjectile : MonoBehaviour
{

    public GameObject explosion;
    public float explosionRadius;
    public float damage;
    public float tempModifier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        var explosionInstance = Instantiate(explosion,transform.position, transform.rotation);

        ApplyDamage();

        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        var surroundingTargets = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider entity in surroundingTargets)
        {
            var register = entity.gameObject.GetComponent<Register>();

            if (register != null)
            {
                register.TakeDamage(null, damage, tempModifier);    // Will have to replace null with transform of furnace by having
                                                                    // furnace pass that info onto projectile
            }
        }
    }
}

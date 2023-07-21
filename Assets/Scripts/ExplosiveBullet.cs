using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Instantiated by script ArcingProjectileAttack.cs- applies AOE damage when hitting a collider and instantiates an explosion script 
 *  - explosion script just for visual effects- damage is calculated in this script
 */

public class ExplosiveBullet : MonoBehaviour
{

    public GameObject explosion;

    public float explosionRadius;
    public float damage;
    public float tempModifier;

    // If bullet doesn't use gravity
    public float lifeTime; 
    public bool hasGravity;  

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasGravity && lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                ApplyDamage();
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ApplyDamage();
        Destroy(gameObject);
    }

    private void ApplyDamage()
    {
        var explosionInstance = Instantiate(explosion, transform.position, transform.rotation);

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

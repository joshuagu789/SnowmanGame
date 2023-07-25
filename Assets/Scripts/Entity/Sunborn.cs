using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Interacts only with Entity script to manage the unique characteristics that sunborn entities have
 *  - similar to robot script except no lock-on lifetime (will hunt target until one or the other is defeated)
 *  - similar to snowman script except higher temperature is good for sunborn while lower temperature harms it
 */

public class Sunborn : MonoBehaviour
{
    public Entity entity;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        if (entity.type.Equals("Enemy"))
        {
            entity.server.enemiesList.Add(transform);
        }
        else if (entity.type.Equals("Snowman"))
        {
            entity.server.snowmenList.Add(transform);
        }
    }

    private void OnDisable()
    {
        if (entity.type.Equals("Enemy"))
        {
            entity.server.enemiesList.Remove(transform);
        }
        else if (entity.type.Equals("Snowman"))
        {
            entity.server.snowmenList.Remove(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStats();
        if (entity.systemIntegrity <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateStats()
    {

        if (entity.register.hasTakenDamage)
        {
            entity.animator.SetTrigger("Dodge");
            entity.systemIntegrity -= entity.register.damageTaken;
            entity.register.hasTakenDamage = false;
        }

    }
}

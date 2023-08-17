using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Universal script for entities to find the closest enemy out of list of enemies currently existing in game
 */

public class EntityTargeting : MonoBehaviour
{

    public Entity entity;
    private List<Transform> targetList = new List<Transform>();

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.type.Equals("Enemy"))
        {
            targetList = entity.server.snowmenList;
        }
        else if (entity.type.Equals("Snowman"))
        {
            targetList = entity.server.enemiesList;
        }

        timer += Time.deltaTime;
        if (!entity.isLockedOn && entity.target == null && timer >= 1f)  // Making entity search for target every second 
        {
            timer = 0f;
            FindClosestTarget();
        }
    }

    void FindClosestTarget()
    {
        /*  
            The code below goes through a loop to find the closest enemy in range
                - the way it's done below is inefficient (does through entire list, runs at all times)
                 but will be improved on if performance drops
        */

        bool targetFound = false;
        Transform closestTarget = null;
        float minDist = Mathf.Infinity;

        foreach (Transform potentialTarget in targetList)
        {
            float distance = new Vector3(potentialTarget.position.x - transform.position.x, 0f, potentialTarget.position.z - transform.position.z).magnitude;
            if (distance < minDist && distance <= entity.detectionRange)
            {
                closestTarget = potentialTarget;
                minDist = distance;
                targetFound = true;
            }
        }

        // Deciding if entity found a potential target
        if (targetFound)
            entity.FocusFire(closestTarget);
        else 
        {
            entity.isLockedOn = false;
            entity.target = null;
            entity.animator.SetBool("isLockedOn", false);
        }
    }
}

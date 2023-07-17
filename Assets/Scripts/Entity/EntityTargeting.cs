using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTargeting : MonoBehaviour
{
    public Entity entity;
    public GameServer server;
    private List<Transform> targetList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.type.Equals("Enemy"))
        {
            targetList = server.snowmenList;
        }
        else if (entity.type.Equals("Snowman"))
        {
            targetList = server.enemiesList;
        }

        if (!entity.isLockedOn)
        {
            StartCoroutine(FindClosestTarget());
        }
    }

    IEnumerator FindClosestTarget()
    {
        bool targetFound = false;

        /*  
           The code below goes through a loop to find the closest snowman in range
            - the way it's done below is inefficient (does through entire list, runs at all times)
              but will be improved on if performance drops
        */

        Transform closestTarget = null;
        float minDist = Mathf.Infinity;

        foreach (Transform potentialTarget in targetList)
        {
            float distance = Vector3.Distance(potentialTarget.position, transform.position);
            if (distance < minDist && distance <= entity.detectionRange)
            {
                closestTarget = potentialTarget;
                minDist = distance;
                targetFound = true;
            }
        }

        // Deciding if enemy found a potential target
        if (targetFound)
        {
            entity.isLockedOn = true;
            entity.target = closestTarget;
            entity.animator.SetBool("isLockedOn", true);
        }
        else
        {
            entity.isLockedOn = false;
            entity.target = null;
            entity.animator.SetBool("isLockedOn", false);
        }

        // Makes entity find closest target every 1f seconds instead of every frame to conserve CPU
        yield return new WaitForSeconds(1f);
    }
}

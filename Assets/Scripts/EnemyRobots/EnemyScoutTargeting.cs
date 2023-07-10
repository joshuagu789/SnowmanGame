using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScoutTargeting : MonoBehaviour
{
    public EnemyRobot entity;
    public GameServer server;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestTarget();
    }

    void FindClosestTarget()
    {
        bool targetFound = false;

        /*  
           The code below goes through a loop to find the closest snowman in range
            - the way it's done below is inefficient (does through entire list, runs at all times)
              but will be improved on if performance drops
        */

        Transform closestTarget = null;
        float minDist = Mathf.Infinity;

        foreach (Transform potentialTarget in server.snowmenList)
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
        }
        else
        {
            entity.isLockedOn = false;
            entity.target = null;
        }
    }
}

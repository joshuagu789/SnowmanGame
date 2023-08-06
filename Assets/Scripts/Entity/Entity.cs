using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   // Needed for NavMeshAgent
using static UnityEngine.EventSystems.EventTrigger;

/*
   Universal script for any game character that isn't the player (npc)- this script only has
   info-carrying variables that other scripts will use
*/

public class Entity : MonoBehaviour
{

    public GameServer server;
    public Register register;
    public Animator animator;
    public NavMeshAgent agent;
    public string type;     // Can be either "Enemy" or "Snowman"

    public float systemIntegrity;
    public float maxIntegrity;

    public float temperature;
    public float minTemperature;

    public float energy;    // Attacks, repairs, dodges, & other actions use up energy
    public float maxEnergy;

    public float speed;
    public float rotationSpeed;

    // For targeting & distance measuring
    public float detectionRange;
    public float range;     // Maximum distance from target from which entity can act
    public float fov;       // AKA Field of View- max degrees to either side of forward where entity can spot/attack enemies

    // Below two variables are used for strafing enemies
    public float minRange;      // Minimum distance from target that entity can be (kind of like tether range)
    public float maxRange;      // Maximum distance from target that entity can be

    public float leashRange;    // Maximum distance from leader that entity can be (usually for snowmen & player)
    public Transform target = null;

    // For the robot's current state
    public bool isIdle = false;
    public bool isLockedOn = false;
    public float lockDuration;
    [HideInInspector]
    public bool isDisabled;

    // For targeting and aiming
    //[HideInInspector]
    public Vector3 distanceToTarget;
    //[HideInInspector]
    public float angleToTarget;

    // For simulating leadership
    public Transform leader;
    public bool isLeader;
    public bool canJoinSquad;

    private float timer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        if (isLockedOn & target != null) 
        {
            UpdateVectors();
        }
    }

    public void AddToServer()
    {
        if (type.Equals("Enemy"))
        {
            server.enemiesList.Add(transform);
        }
        else if (type.Equals("Snowman"))
        {
            server.snowmenList.Add(transform);
        }
    }

    public void RemoveFromServer()
    {
        if (type.Equals("Enemy"))
        {
            server.enemiesList.Remove(transform);
        }
        else if (type.Equals("Snowman"))
        {
            server.snowmenList.Remove(transform);
        }
    }

    public void UpdateVectors()
    {
        timer += Time.deltaTime;

        if (timer >= 0.25f)  // UpdateVectors() will execute every 0.25 seconds
        {
            timer = 0f;
            distanceToTarget = new Vector3(target.position.x - transform.position.x, 0f, target.position.z - transform.position.z);
            angleToTarget = Vector3.Angle(transform.forward, distanceToTarget);
        }
    }
}
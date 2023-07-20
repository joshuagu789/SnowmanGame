using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   // Needed for NavMeshAgent

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

    public float integrityRegen;
    public float integrityLoss;
    public float tempGain;
    public float tempLoss;

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
    public bool isMoving = false;
    public bool isIdle = false;
    public bool isLockedOn = false;
    public float lockDuration;

    // Start is called before the first frame update
    void Start()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;
    }
}
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
    [HideInInspector]
    public float defaultLeashRange;
    public Transform target = null;

    // For the entity's current state
    public bool isIdle = false;
    public bool isLockedOn = false;
    public float lockDuration;
    [HideInInspector]
    public bool isDisabled;
    [HideInInspector]
    public bool walkPointSet;   // Means the entity's NavMeshAgent has a destination
    private Vector3 walkPoint;

    // For targeting and aiming
    [HideInInspector]
    public Vector3 vectorToTarget;
    [HideInInspector]
    public float distanceToTargetSqr;   // Is the squared distance since taking sqrRoot is costly- better to compare squared values w/ squared values
    [HideInInspector]
    public float angleToTarget;

    // For simulating leadership
    public Transform leader;
    public bool isLeader;
    public bool canJoinSquad;
    public List<Entity> squadList = new List<Entity>();

    private float timer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        defaultLeashRange = leashRange;
    }

    private void Update()
    {
        agent.speed = speed;
        agent.angularSpeed = rotationSpeed;

        UpdateVectors();
    }

    public void OnDestroy()
    {
        if (leader != null)
            leader.GetComponent<Entity>().squadList.Remove(this);
    }

    public void AddToServer()
    {
        if (type.Equals("Enemy") && server != null)
        {
            server.enemiesList.Add(transform);
        }
        else if (type.Equals("Snowman") && server != null)
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

    public virtual void UpdateVectors()
    {
        if (target != null && isLockedOn)
        {
            timer += Time.deltaTime;

            if (timer >= 0.25f)  // UpdateVectors() will execute every 0.25 seconds
            {
                timer = 0f;
                vectorToTarget = new Vector3(target.position.x - transform.position.x, 0f, target.position.z - transform.position.z);
                angleToTarget = Vector3.Angle(transform.forward, vectorToTarget);
                distanceToTargetSqr = vectorToTarget.sqrMagnitude;
            }
        }
    }

    // Below methods are for commands from other entities or for itself if it has the right script for it
    public void FocusFire(Transform target) { isLockedOn = true; this.target = target; animator.SetBool("isLockedOn", true); }

    public void IncrementLeashRange(int increment)
    {
        if (increment * defaultLeashRange + leashRange >= agent.radius){ leashRange = increment * defaultLeashRange + leashRange; }
    }

    public void MoveTo(Vector3 location)
    {
        isIdle = false;
        walkPointSet = true;
        walkPoint = location;
    }

    public Vector3 GetWalkPoint() { return walkPoint; }
}
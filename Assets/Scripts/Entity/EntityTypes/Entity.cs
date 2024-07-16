using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;   // Needed for NavMeshAgent
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// Script given to any interactable game object that can be destroyed. Entity is the parent script for other game characters such as
/// snowmen, robots, the player, and even buildings.
/// <br/>
/// Typical traits: can move, can be damaged, can be assigned a leader or be a leader
/// <br/>
/// Typical parter scripts: movement, target locking, attack
/// </summary>
/// <param name="param1">Some Parameter.</param>
/// <returns>What this method returns.</returns>

public enum EntityType {
    ENEMY,
    SNOWMAN,
    RESOURCE
}

public class Entity : MonoBehaviour
{

    public GameServer server;
    public Register register;
    public Animator animator;
    public NavMeshAgent agent;
    [SerializeField]
    private EntityType type;
    public float systemIntegrity;
    public float maxIntegrity;

    public float temperature;

    public float energy;    // Attacks, repairs, dodges, & other actions use up energy
    public float maxEnergy;

    public float speed;
    public float rotationSpeed;

    // For targeting & distance measuring
    public float detectionRange;
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
        if (agent != null)
        {
            agent.speed = speed;
            agent.angularSpeed = rotationSpeed;
        }

        UpdateVectors();
    }

    public virtual void OnDestroy()
    {
        if (leader != null)
            leader.GetComponent<Entity>().squadList.Remove(this);
    }

    /**
     * Removes entity from the game but doesn't destroy game object 
     */
    public virtual void DieNotPermanent() {
        if (leader != null)
            leader.GetComponent<Entity>().squadList.Remove(this);
        RemoveFromServer();
        server.ClearLocksOn(this);
        gameObject.SetActive(false);
    }

    public void AddToServer()
    {
        print(GetComponent<Entity>());
        //server.AddToServer(this);
        server.AddToServer(GetComponent<Entity>());
    }

    public void RemoveFromServer()
    {
        server.RemoveFromServer(GetComponent<Entity>());
    }

    public virtual void CheckDamage()
    {
        if (register.HasTakenDamage())
        {
            // Makes entity target its attacker if the attack allows for it
            if (register.GetDamageSource() != null && !register.GetDamageSource().gameObject.GetComponentInParent<Entity>().type.Equals(type))
            {
                FocusFire(register.GetDamageSource());
            }
            systemIntegrity -= register.GetDamageTaken();
            temperature += register.GetTempIncrease();
            register.ResetRegister();
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

    public virtual void UpdateLockState()
    {
        if (target == null || !isLockedOn)
        {
            isLockedOn = false;
            animator.SetBool("isLockedOn", false);
        }
    }

    // ENTITY COMMANDS
    public EntityType GetEntityType() { return type; }

    // Below methods are for commands from other entities or for itself if it has the right script for it
    public void FocusFire(Transform target)
    {
        isLockedOn = true;
        this.target = target;
        animator.SetBool("isLockedOn", true);
        timer = 1f;
        UpdateVectors();    // Instantly getting data for distance to new target to avoid weird stuff from happening (such as melee attacking enemy 50 m away)
    }

    public void ClearTarget()
    {
        isLockedOn = false;
        target = null;
        UpdateLockState();
    }

    public void IncrementLeashRange(int increment)
    {
        if (increment * defaultLeashRange + leashRange >= agent.radius){ leashRange = increment * defaultLeashRange + leashRange; }
    }

    public virtual void MoveTo(Vector3 location)
    {
        isIdle = false;
        walkPointSet = true;
        walkPoint = location;
        animator.SetBool("isMoving", true);
        agent.SetDestination(location);
    }

    public virtual void StandStill()
    {
        agent.ResetPath();
        walkPointSet = false;
        walkPoint = new Vector3(0f, 0f, 0f);
        animator.SetBool("isMoving", false);
    }

    // Similar to StandStill() but used as an attack mechanic
    public virtual void Root()
    {
        if (agent != null) {
            agent.isStopped = true;
        }    
    }

    public virtual void Unroot()
    {
        if (agent != null)
            agent.isStopped = false;
    }

    // Like Root but has a duration
    public virtual void RootFor(float duration)
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private IEnumerator StopFor(float duration) {
        if (agent != null) {
            agent.isStopped = true;
            yield return new WaitForSeconds(duration);
            agent.isStopped = false;
        }
    }

    public void FaceLocation(Vector3 location)
    {
        var targetRotation = Quaternion.LookRotation(new Vector3(location.x - transform.position.x, 0f, location.z - transform.position.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed / 360 * Time.deltaTime);
    }

    // GETTERS AND SETTERS
    // Shuts down entity as well as any child entities that it has if a is false, and enables if true
    public virtual void SetDisableAll(bool a)
    {
        var entities = GetComponentsInChildren<Entity>();
        foreach (Entity entity in entities)
            entity.SetIsDisabled(a);
    }

    public bool GetIsDisabled() { return isDisabled; }
    public void SetIsDisabled(bool a) { isDisabled = a; }
    public Vector3 GetWalkPoint() { return walkPoint; }
    public Entity GetTarget()
    {
        if(target != null)
            return target.GetComponent<Entity>();
        return null;
    }
    public float GetAngleToTarget() { return angleToTarget; }
}
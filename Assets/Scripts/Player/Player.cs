using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * First script created to encompass all smaller player scripts as well as holding vital player info
 *  - Later replace this with the Entity script?
 */

public class Player : MonoBehaviour
{

    public Animator animator;
    public GameServer server;
    public Register register;

    // Starting stats of the player
    public float systemIntegrity = 100f;
    public float maxIntegrity = 100f;

    public float temperature = 0f;
    public float minTemperature = 0f;

    public float integrityRegen = 1f;
    public float integrityLoss = 0f;
    public float tempGain = 0f;
    public float tempLoss = 1f;

    public float speed = 6f;

    public bool isLockedOn = false;
    public Transform target;
    public float detectionRange = 100f;

    public List<Transform> squadList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        server.snowmenList.Add(transform);
    }

    private void OnDisable()
    {
        server.snowmenList.Remove(transform);
    }

    // Update is called once per frame
    void Update()
    {
        CheckMelt();
        UpdateStats();
    }

    void UpdateStats()
    {
        // Calculating how to change integrity (aka health) and temperature
        float tempChange = tempGain - tempLoss;     
        temperature += tempChange * Time.deltaTime;
        temperature = Mathf.Clamp(temperature, minTemperature, Mathf.Infinity);

        float integrityChange = integrityRegen - integrityLoss - temperature / 10f;
        systemIntegrity += integrityChange * Time.deltaTime;
        systemIntegrity = Mathf.Clamp(systemIntegrity, 0, maxIntegrity);

        if (register.hasTakenDamage)
        {
            systemIntegrity -= register.damageTaken;
            register.hasTakenDamage = false;
        }
    }

    public void CheckMelt()
    {
        // If player is going to melt
        if (systemIntegrity <= 0f)
        {
            animator.SetBool("isMelting", true);
            speed = 1f;
        }
        /*
        else
        {
            animator.SetBool("isMelting", false);
            speed = 6f;
        }
        */
    }
    
}

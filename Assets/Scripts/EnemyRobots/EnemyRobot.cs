using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRobot : MonoBehaviour
{
    public GameServer server;

    public float integrity;

    // For targeting
    public float detectionRange;
    public float minRange;
    public float maxRange;
    public Transform target = null;

    // For the robot's current state
    public bool isMoving = false;
    public bool isIdle = false;
    public bool isLockedOn = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        server.enemiesList.Add(transform);
    }

    private void OnDisable()
    {
        server.enemiesList.Remove(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

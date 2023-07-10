using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowman : MonoBehaviour
{
    public GameServer server;

    // Starting stats of snowman
    public float systemIntegrity;
    public float maxIntegrity;

    public float temperature;
    public float minTemperature;

    public float integrityRegen;
    public float integrityLoss;
    public float tempGain;
    public float tempLoss;

    public static float speed;

    // Start is called before the first frame update
    void Start()
    {

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
        
    }
    
}

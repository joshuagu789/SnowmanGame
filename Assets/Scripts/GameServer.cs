using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServer : MonoBehaviour
{
    // Lists to keep track of all active entities
    public List<Transform> snowmenList = new List<Transform>();   // Includes player
    public List<Transform> enemiesList = new List<Transform>();

    public float threatLevel = 0f;
    public float gameTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // To make gameTime reflect seconds passed since game started
        gameTime += Time.deltaTime;
    }
}
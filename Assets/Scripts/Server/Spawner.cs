/*
 * Scripts designed for instantiating and placing down enemies/allies down
 *  - gets called on by game server
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameServer gameServer;
    public Transform player;

    public List<GameObject> tierOneEnemies = new List<GameObject>();
    public List<GameObject> tierTwoEnemies = new List<GameObject>();

    public List<GameObject> tierOneSnowmen = new List<GameObject>();
    public List<GameObject> tierTwoSnowmen = new List<GameObject>();

    private SortedDictionary<int, List<GameObject>> enemiesList = new SortedDictionary<int, List<GameObject>>();
    private SortedDictionary<int, List<GameObject>> snowmenList = new SortedDictionary<int, List<GameObject>>();

    private SortedDictionary<int, List<GameObject>> entityList;
    private Vector3 spawnLocation;

    // Start is called before the first frame update
    void Awake()
    {
        enemiesList.Add(1, tierOneEnemies);
        enemiesList.Add(2, tierTwoEnemies);
        snowmenList.Add(1, tierOneSnowmen);
        snowmenList.Add(2, tierTwoSnowmen);
    }

    // Instantiates random entities from a list which is determined by type and tier (type being either "Snowmen" or "Enemy")
    public void SpawnRandom(string type, int minTier, int maxTier,int amount, Transform location, int minRange, int maxRange)
    {
        if (amount > 0)
        {
            ChooseList(type);
            SelectSpawnLocation(location, minRange, maxRange, 10);

            for (int i = 0; i < amount; i++)
            {
                var tierList = entityList[(int)Random.Range(minTier, maxTier + 1)]; // Selecting random tier to spawn units from and assigning the list containing the units to variable
                var index = Random.Range(0, tierList.Count);    // Choosing random unit within the list of units corresponding to a tier
                var spawnedEntity = Instantiate(tierList[index], spawnLocation, transform.rotation);

                spawnedEntity.GetComponent<Entity>().server = gameServer;
                spawnedEntity.GetComponent<Entity>().AddToServer();
            }
        }
    }

    // Same as SpawnRandom() but all entities spawned are the same
    public void SpawnSpecific(string type, int tier, int amount, Transform location, int minRange, int maxRange)
    {
        ChooseList(type);
    }

    // Same as SpawnRandom() but first entity created is a leader to which the rest of the spawned entities are assigned to follow 
    public void SpawnSquad(string type, int tier, int amount, Transform location, int minRange, int maxRange)
    {
        ChooseList(type);
    }

    private void ChooseList(string type)
    {
        if (type.Equals("Enemy") || type.Equals("Enemies") || type.Equals("enemy") || type.Equals("enemies"))
            entityList = enemiesList;
        else if (type.Equals("Snowman") || type.Equals("Snowmen") || type.Equals("snowman") || type.Equals("snowmen") || type.Equals("Ally") || type.Equals("ally") || type.Equals("Allies") || type.Equals("allies"))
            entityList = snowmenList;
    }

    private void SelectSpawnLocation(Transform location, int minRange, int maxRange, int numberOfTries)
    {
        Vector3 spawnCenter;
        if (location != null)
            spawnCenter = location.position;
        else
            spawnCenter = new Vector3(0f,0f,0f);

        Mathf.Clamp(minRange, 0, Mathf.Infinity);
        Mathf.Clamp(maxRange, 0, Mathf.Infinity);
        // Choose a random spawn location that is between minRange and maxRange from location
        spawnLocation = Quaternion.AngleAxis(Random.Range(0, 360), location.transform.up) * -location.position.normalized * Random.Range(minRange, maxRange) + location.position;

        // Making sure spawn location is a valid place to instantiate entities
        RaycastHit hit;
        if (Physics.Raycast(spawnLocation, transform.up, out hit, 1000) || Physics.Raycast(spawnLocation, -transform.up, out hit, 1000))    // If spawn location is below/above a collider (collider could potentially be the ground)
        {
            // Retrying method if spawn location is not on ground
            if (!hit.collider.gameObject.tag.Equals("Ground") && numberOfTries > 0)
                SelectSpawnLocation(location, minRange, maxRange, numberOfTries - 1);
            else
                spawnLocation = hit.point;  // Placing spawnLocation exactly on the point where the ray hit the ground
        }
        else if (numberOfTries > 0)
            SelectSpawnLocation(location, minRange, maxRange, numberOfTries - 1);
        else
            spawnLocation = new Vector3(0f,0f,0f);  // Sets spawn location to world origin if it can't find a good place to spawn after numberOfTries iterations
    }
}
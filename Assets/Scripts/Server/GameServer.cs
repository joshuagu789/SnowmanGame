using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Script to hold information about the game's state as well as for starting events (such as spawning enemies)
 *  - Later put list of entities all into a hashmap rather than separate lists of transforms?
 */

public class GameServer : MonoBehaviour
{
    // Lists to keep track of all active entities
    public List<Transform> snowmenList = new List<Transform>();   // Includes player
    public List<Transform> enemiesList = new List<Transform>();

    public Spawner spawner;
    public Player player;

    private float detectionLevel = 0f;
    public float detectionThreshold;
    public float threatLevel = 0f;

    private int maxEntities = 100;
    private float gameTime = 0f;
    private float sunRiseTime = 600f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            spawner.SpawnRandom("Enemies", (int)Random.Range(1, 2), (int)Random.Range(2, 5), player.transform, 500, 1000);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // To make gameTime reflect seconds passed since game started
        gameTime += Time.deltaTime;

        if (detectionLevel >= detectionThreshold && snowmenList.Count + enemiesList.Count <= maxEntities)
        {
            detectionLevel = 0;
            StartCoroutine(StartWaveEvent(6f));
            print("spawning wave");
        }
    }

    private IEnumerator StartWaveEvent(float time)
    {
        MoveAllEnemies(player.transform.position);
        yield return new WaitForSeconds(time);

        for (int i = 0; i < 3; i++)
        {
            spawner.SpawnRandom("Enemies",(int)Random.Range(1,2),(int)Random.Range(2,5),player.transform,300,500);
        }

        MoveAllEnemies(player.transform.position);
    }

    private void MoveAllEnemies(Vector3 location)
    {
        foreach (Transform enemy in enemiesList)
        {
            enemy.GetComponent<Entity>().MoveTo(new Vector3(location.x + Random.Range(-150f, 150f), location.y, location.z + Random.Range(-150f, 150f)));
        }
    }

    public void RaiseDetectionLevel(float amount) { detectionLevel += amount; }
}

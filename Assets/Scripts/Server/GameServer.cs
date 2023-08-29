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
    public List<Transform> resourcesList = new List<Transform>();

    public BroadcastDialogue broadcaster;
    public MusicManager music;
    public Spawner spawner;
    public Player player;

    private float detectionLevel = 0f;
    public float detectionThreshold;
    private float waveCooldown = 180f;
    public float threatLevel = 0f;

    private int maxEntities = 100;
    //private float sunRiseTime = 600f;
    private float cooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        cooldownTimer = waveCooldown;
        for (int i = 0; i < 12; i++)
        {
            spawner.SpawnRandom("Enemies", 1, 2, (int)Random.Range(2, 4), player.transform.position, 500, 1000);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) { detectionLevel += 0.3f; }

        cooldownTimer += Time.deltaTime;

        if (detectionLevel >= detectionThreshold && cooldownTimer >= waveCooldown)
        {
            cooldownTimer = 0;
            detectionLevel = 0;
            StartCoroutine(StartWaveEvent(6f));
            broadcaster.StartWaveDialogue();
            music.PlayWaveMusic();
        }
    }

    private IEnumerator StartWaveEvent(float time)
    {
        MoveAllEnemies(player.transform.position);
        yield return new WaitForSeconds(time);

        if (snowmenList.Count + enemiesList.Count <= maxEntities)
        {
            for (int i = 0; i < 3; i++)
            {
                spawner.SpawnRandom("Enemies", 1, 3, (int)Random.Range(2, 5), player.transform.position, 300, 500);
            }
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

    public void RaiseDetectionLevel(float amount)
    {
        if(cooldownTimer >= waveCooldown)
            detectionLevel += amount;
    }
}

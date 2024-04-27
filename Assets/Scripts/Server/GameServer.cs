using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Script to hold information about the game's state as well as for starting events (such as spawning enemies)
 *  - The only way entities can be interacted with is if they are recorded inside the game server
 */

public class GameServer : MonoBehaviour
{
    // Lists to keep track of all active entities
    public HashSet<Entity> snowmenList = new HashSet<Entity>();   // Includes player
    public HashSet<Entity> enemiesList = new HashSet<Entity>();
    public HashSet<Entity> resourcesList = new HashSet<Entity>();

    public BroadcastDialogue broadcaster;
    public MusicManager music;
    public Spawner spawner;
    public Player player;

    public float detectionLevel = 0f;   //temporarily public
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
                spawner.SpawnRandom("Enemies", 2, 3, (int)Random.Range(2, 5), player.transform.position, 300, 500);
            }
        }

        MoveAllEnemies(player.transform.position);
    }

    private void MoveAllEnemies(Vector3 location)
    {
        foreach (Entity enemy in enemiesList)
        {
            enemy.MoveTo(new Vector3(location.x + Random.Range(-150f, 150f), location.y, location.z + Random.Range(-150f, 150f)));
        }
    }

    public void RaiseDetectionLevel(float amount)
    {
        if (cooldownTimer >= waveCooldown)
            detectionLevel += amount;
    }

    /// <returns>True if game server successfully added entity, false if entity is already in game server.</returns>
    public bool AddToServer(Entity entity)
    {
        HashSet<Entity> targetGroup = null;
        if (entity.GetEntityType().Equals(EntityType.SNOWMAN))
            targetGroup = snowmenList;
        else if (entity.GetEntityType().Equals(EntityType.ENEMY))
            targetGroup = enemiesList;
        else if (entity.GetEntityType().Equals(EntityType.RESOURCE))
            targetGroup = resourcesList;

        if (targetGroup == null || targetGroup.Contains(entity))
            return false;
        targetGroup.Add(entity);
        return true;
    }

    /// <returns>True if game server successfully removed entity, false if entity wasn't in game server to begin with.</returns>
    public bool RemoveFromServer(Entity entity)
    {
        HashSet<Entity> targetGroup = null;
        if (entity.GetEntityType().Equals(EntityType.SNOWMAN))
            targetGroup = snowmenList;
        else if (entity.GetEntityType().Equals(EntityType.ENEMY))
            targetGroup = enemiesList;
        else if (entity.GetEntityType().Equals(EntityType.RESOURCE))
            targetGroup = resourcesList;

        if (targetGroup == null || !targetGroup.Contains(entity))
            return false;
        targetGroup.Remove(entity);
        ClearLocksOn(entity);
        return true;
    }

    /// <summary>
    /// Removes all friendly and enemy locks on entity- typically to prevent bugs when moving/deleting/hiding entity 
    public void ClearLocksOn(Entity entity)
    {
        foreach (Entity elem in snowmenList)
            if (elem.GetTarget() != null && elem.GetTarget().Equals(entity))
                elem.ClearTarget();

        foreach (Entity elem in enemiesList)
            if (elem.GetTarget() != null && elem.GetTarget().Equals(entity))
                elem.ClearTarget();
    }
}
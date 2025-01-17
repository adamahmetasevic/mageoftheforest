using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawner Settings")]
    public List<Transform> spawnPoints = new List<Transform>();
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    public float spawnRadius = 10f;

    [Header("Enemy Spawn Assignments")]
    public List<int> enemyAssignments = new List<int>();

    private Dictionary<Transform, GameObject> spawnPointToEnemy = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, bool> playerInRange = new Dictionary<Transform, bool>();
    private Transform player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        // Initialize spawn points
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            spawnPointToEnemy[spawnPoints[i]] = null;
            playerInRange[spawnPoints[i]] = false;
            SpawnEnemyAt(spawnPoints[i], enemyAssignments[i]);
        }
    }

    private void Update()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            bool wasInRange = playerInRange[spawnPoint];
            bool isInRange = Vector2.Distance(player.position, spawnPoint.position) <= spawnRadius;

            if (isInRange && !wasInRange)
            {
                // Player just entered the range
                playerInRange[spawnPoint] = true;
                CheckAndSpawnEnemy(spawnPoint);
            }
            else if (!isInRange && wasInRange)
            {
                // Player just left the range
                playerInRange[spawnPoint] = false;
            }
        }
    }

    private void CheckAndSpawnEnemy(Transform spawnPoint)
    {
        if (!spawnPointToEnemy.ContainsKey(spawnPoint))
        {
            Debug.LogError("Spawn point not initialized in dictionary.");
            return;
        }

        GameObject existingEnemy = spawnPointToEnemy[spawnPoint];

        if (existingEnemy == null || !existingEnemy.activeSelf)
        {
            int index = spawnPoints.IndexOf(spawnPoint);
            SpawnEnemyAt(spawnPoint, enemyAssignments[index]);
        }
    }

    private void SpawnEnemyAt(Transform spawnPoint, int enemyIndex)
    {
        if (enemyIndex < 0 || enemyIndex >= enemyPrefabs.Count)
        {
            Debug.LogError("Invalid enemy index.");
            return;
        }

        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPoint.position, Quaternion.identity);
        spawnPointToEnemy[spawnPoint] = enemy;

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeathTriggered += () => HandleEnemyDeath(spawnPoint);
        }
    }

    private void HandleEnemyDeath(Transform spawnPoint)
    {
        spawnPointToEnemy[spawnPoint] = null;
    }
}
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject fireEnemyPrefab;
    public Transform[] spawnPoints;

    public void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Instantiate(fireEnemyPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
    }
}

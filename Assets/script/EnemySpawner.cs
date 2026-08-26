using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    
    [SerializeField] private float initialSpawnInterval = 5f; // Start spawning every 5 seconds
    [SerializeField] private float minSpawnInterval = 1f; // Minimum spawn every 1 second (cap)
    [SerializeField] private float difficultyIncreasePerSecond = 0.01f; // How fast difficulty increases
    
    private float spawnTimer = 0f;
    private float currentSpawnInterval;
    private float elapsedTime = 0f;
    private bool isSpawning = true;

    void Start()
    {
        currentSpawnInterval = initialSpawnInterval;
        
      
    }

    void Update()
    {
        if (!isSpawning) return;
        
        elapsedTime += Time.deltaTime;
        spawnTimer += Time.deltaTime;
        
        // Decrease spawn interval as time progresses (increase difficulty)
        currentSpawnInterval = Mathf.Max(
            minSpawnInterval, 
            initialSpawnInterval - (elapsedTime * difficultyIncreasePerSecond)
        );
        
        // Spawn when timer reaches interval
        if (spawnTimer >= currentSpawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
        
       
    }

    void SpawnEnemy()
    {
        // Get random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        
        // Get random enemy prefab
        GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        
        // Spawn the enemy
        Instantiate(randomEnemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        
        
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void ResumeSpawning()
    {
        isSpawning = true;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public float GetCurrentSpawnInterval()
    {
        return currentSpawnInterval;
    }
}
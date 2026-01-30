using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.5f;
    public float spawnRadius = 12f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Vector2 spawnPos = Random.insideUnitCircle.normalized * spawnRadius;
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}

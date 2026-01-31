using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    public float spawnRadius = 12f; // Distance from player to spawn

    private float timer;
    private Transform playerTransform;

    void Update()
    {
        // 1. Find the player if we don't have them yet
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            return; // Don't spawn until player is found
        }

        // 2. Standard timer logic
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemyAroundPlayer();
            timer = 0f;
        }
    }

    void SpawnEnemyAroundPlayer()
    {
        // 3. Create a random point on a circle
        // Random.insideUnitCircle.normalized gives us a point exactly 1 unit away in a random direction
        Vector2 randomDirection = Random.insideUnitCircle.normalized;

        // Multiply by our radius (e.g., 12 units away)
        Vector2 spawnOffset = randomDirection * spawnRadius;

        // Add the player's current position so the enemy spawns relative to them
        Vector3 spawnPosition = playerTransform.position + (Vector3)spawnOffset;

        // 4. Spawn the enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
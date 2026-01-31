using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnEffectPrefab; // Drag Spawn_FX here
    public float spawnInterval = 3f;
    public float spawnRadius = 12f;
    public float spawnDelay = 0.2f; // Short delay for the effect to play

    private float timer;
    private Transform playerTransform;

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            return;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            StartCoroutine(SpawnRoutine());
            timer = 0f;
        }
    }

    IEnumerator SpawnRoutine()
    {
        // 1. Calculate Position
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = playerTransform.position + (Vector3)(randomDirection * spawnRadius);

        // 2. Play Spawn Effect
        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
        }

        // 3. Wait slightly (so the flash happens before the enemy pops in)
        yield return new WaitForSeconds(spawnDelay);

        // 4. Spawn Enemy
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}
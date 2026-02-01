using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnEffectPrefab;

    [Header("Spawn Timing")]
    public float spawnInterval = 1.5f;
    public float spawnDelay = 0.3f;

    [Header("Exclusion Settings")]
    public LayerMask forbiddenLayer;
    public float checkRadius = 0.5f;

    [Header("Wave Limits")]
    public int maxToSpawn = 10; // SET THIS IN INSPECTOR: How many trees total?
    private int spawnedCount = 0;
    private int killedCount = 0;

    private float timer;
    private bool isPlayerInside = false;

    void Update()
    {
        // Only run logic if player is inside AND we haven't hit the spawn limit
        if (isPlayerInside && spawnedCount < maxToSpawn)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                StartCoroutine(SpawnRoutine());
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }

        // Shutdown the spawner entirely once every tree spawned is dead
        if (spawnedCount >= maxToSpawn && killedCount >= maxToSpawn)
        {
            Debug.Log("Cursed Woods Cleared! Spawner Deactivated.");
            gameObject.SetActive(false);
        }
    }

    IEnumerator SpawnRoutine()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool foundValidSpot = false;
        int maxAttempts = 10;
        int currentAttempt = 0;

        while (!foundValidSpot && currentAttempt < maxAttempts)
        {
            float randomX = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
            float randomY = Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2);
            spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

            Collider2D hit = Physics2D.OverlapCircle(spawnPosition, checkRadius, forbiddenLayer);
            if (hit == null) foundValidSpot = true;
            currentAttempt++;
        }

        if (foundValidSpot)
        {
            spawnedCount++; // Increment count here
            if (spawnEffectPrefab != null)
            {
                Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnDelay);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // New function for the Enemy to call
    public void RegisterKill()
    {
        killedCount++;
        Debug.Log($"Enemy Defeated. Progress: {killedCount}/{maxToSpawn}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Player entered the Cursed Woods!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isPlayerInside ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
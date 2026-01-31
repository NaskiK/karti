using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnEffectPrefab;

    [Header("Spawn Timing")]
    public float spawnInterval = 1.5f;
    public float spawnDelay = 0.3f;

    [Header("Exclusion Settings (Anti-Stuck)")]
    public LayerMask forbiddenLayer; // Set this to "Forbidden"
    [Tooltip("Increase this if enemies still spawn too close to forbidden objects")]
    public float checkRadius = 1.2f;

    private float timer;
    private bool isPlayerInside = false;

    void Update()
    {
        if (isPlayerInside)
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
    }

    IEnumerator SpawnRoutine()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool foundValidSpot = false;
        int maxAttempts = 15; // Increased attempts to find a clean spot
        int currentAttempt = 0;

        while (!foundValidSpot && currentAttempt < maxAttempts)
        {
            // Calculate a random spot within the spawner's box
            float randomX = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
            float randomY = Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2);
            spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

            // Physics Check: Is there a "Forbidden" collider at this spot?
            // We use OverlapCircle to check a radius around the point
            Collider2D hit = Physics2D.OverlapCircle(spawnPosition, checkRadius, forbiddenLayer);

            if (hit == null)
            {
                foundValidSpot = true;
            }
            currentAttempt++;
        }

        if (foundValidSpot)
        {
            // 1. Show the warning/spawn effect
            if (spawnEffectPrefab != null)
            {
                Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
            }

            // 2. Wait for the delay
            yield return new WaitForSeconds(spawnDelay);

            // 3. Spawn the actual enemy
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            // If it fails 15 times, it just skips this spawn cycle to prevent lag
            Debug.LogWarning("Spawner: No room found! Skipping spawn.");
        }
    }

    // --- TRIGGER DETECTION ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Player entered spawn zone.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            Debug.Log("Player left spawn zone.");
        }
    }

    // This helps you see the check radius in the Scene View
    private void OnDrawGizmos()
    {
        Gizmos.color = isPlayerInside ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
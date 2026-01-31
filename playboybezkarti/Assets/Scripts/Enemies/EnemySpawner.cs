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
    public LayerMask forbiddenLayer; // Set this to "Forbidden" in Inspector
    public float checkRadius = 0.5f; // How big of a 'safe gap' to check for

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
        int maxAttempts = 10; // Don't loop forever if area is full
        int currentAttempt = 0;

        while (!foundValidSpot && currentAttempt < maxAttempts)
        {
            float randomX = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
            float randomY = Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2);
            spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

            // Check if there is anything on the "Forbidden" layer at this spot
            Collider2D hit = Physics2D.OverlapCircle(spawnPosition, checkRadius, forbiddenLayer);

            if (hit == null)
            {
                foundValidSpot = true;
            }
            currentAttempt++;
        }

        // Only spawn if we actually found a safe spot
        if (foundValidSpot)
        {
            if (spawnEffectPrefab != null)
            {
                Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnDelay);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Spawner could not find a valid spot (everything is blocked!)");
        }
    }

    // --- TRIGGER DETECTION ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Player entered the Cursed Woods! Spawning started.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            Debug.Log("Player left the zone. Spawning stopped.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isPlayerInside ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
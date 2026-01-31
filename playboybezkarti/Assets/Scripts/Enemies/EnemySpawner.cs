using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnEffectPrefab;

    [Header("Spawn Timing")]
    public float spawnInterval = 1.5f;
    public float spawnDelay = 0.3f;

    private float timer;
    private bool isPlayerInside = false;

    void Update()
    {
        // Only run the timer and spawn if the player is currently in the zone
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
            // Optional: Reset timer when player leaves so they don't get 
            // an instant spawn the moment they step back in.
            timer = 0f;
        }
    }

    IEnumerator SpawnRoutine()
    {
        float randomX = Random.Range(-transform.localScale.x / 2, transform.localScale.x / 2);
        float randomY = Random.Range(-transform.localScale.y / 2, transform.localScale.y / 2);

        Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0);

        if (spawnEffectPrefab != null)
        {
            Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
        }

        yield return new WaitForSeconds(spawnDelay);
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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
        Gizmos.color = isPlayerInside ? Color.red : Color.green; // Red when active!
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
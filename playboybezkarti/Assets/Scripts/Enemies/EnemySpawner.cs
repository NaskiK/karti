using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnEffectPrefab;

    [Header("Companion Settings")]
    public NPCFollow moleScript; // Drag the CompanionNPC here

    [Header("Spawn Timing")]
    public float spawnInterval = 1.5f;
    public float spawnDelay = 0.3f;

    [Header("Exclusion Settings")]
    public LayerMask forbiddenLayer;
    public float checkRadius = 0.5f;

    [Header("Kill Tracker")]
    public int maxToSpawn = 10;
    private int spawnedCount = 0;
    private int killedCount = 0;

    private float timer;
    private bool isPlayerInside = false;
    private bool hasFinished = false;

    void Update()
    {
        if (hasFinished) return;

        if (isPlayerInside && spawnedCount < maxToSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                StartCoroutine(SpawnRoutine());
                timer = 0f;
            }
        }

        // Check if all enemies are dead
        if (spawnedCount >= maxToSpawn && killedCount >= maxToSpawn)
        {
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        hasFinished = true;
        if (moleScript != null)
        {
            moleScript.LeaveMap(); // Tell the mole to walk through walls and leave
        }
        Debug.Log("Cursed Woods Cleared!");
        // Keep the spawner object for a moment, then disable
        Invoke("DisableSpawner", 0.5f);
    }

    void DisableSpawner() => gameObject.SetActive(false);

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
            spawnedCount++;
            if (spawnEffectPrefab != null) Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void RegisterKill()
    {
        killedCount++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInside = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isPlayerInside ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
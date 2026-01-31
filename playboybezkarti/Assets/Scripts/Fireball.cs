using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;       // Fireball movement speed
    public float lifetime = 2f;     // Auto-destroy after this time

    private Vector3 direction;

    /// <summary>
    /// Initialize the fireball with a direction.
    /// Damage will always come from the player stats.
    /// </summary>
    /// <param name="dir">Direction to move</param>
    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifetime); // Auto destroy
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Get the player damage from the PlayerStats component
                var player = GameObject.FindWithTag("Player"); // Make sure your player has tag "Player"
                if (player != null)
                {
                    var stats = player.GetComponent<PlayerStats>();
                    if (stats != null)
                        enemy.TakeDamage(stats.damage);
                }
            }

            Destroy(gameObject); // Destroy fireball on hit
        }
    }
}

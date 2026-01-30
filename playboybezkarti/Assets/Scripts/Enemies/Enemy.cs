using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 10f;
    public float moveSpeed = 2f;

    float currentHealth;
    Vector2 moveDirection;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // Pick a random direction at start
        moveDirection = Random.insideUnitCircle.normalized;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        // Move in the random direction
        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;

        // Bounce off screen edges (optional)
        Vector3 pos = transform.position;
        if (pos.x > 10f || pos.x < -10f) moveDirection.x *= -1;
        if (pos.y > 5f || pos.y < -5f) moveDirection.y *= -1;
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}

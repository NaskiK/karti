using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 50f;
    private float currentHealth;
    public GameObject deathEffect; // Optional: Drag a particle effect here later

    [Header("Movement Settings")]
    public float speed = 2f;
    public float detectionRange = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public float attackRate = 1.5f;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize HP
        currentHealth = maxHealth;

        if (animator != null) animator.SetBool("IsMoving", false);
    }

    void Update()
    {
        FindPlayerIfNeeded();

        if (player == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else if (distance <= detectionRange)
        {
            MoveToPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    // --- HEALTH LOGIC ---
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("Evil Tree took damage! HP left: " + currentHealth);

        // Play "Hurt" animation if you have one
        if (animator != null) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Evil Tree has been chopped down!");

        // If you have a death effect prefab, spawn it
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
    // --- END HEALTH LOGIC ---

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null) animator.SetBool("IsMoving", true);

        if (direction.x > 0) transform.localScale = new Vector3(2, 2, 2);
        else if (direction.x < 0) transform.localScale = new Vector3(-2, 2, 2);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsMoving", false);

        if (Time.time >= nextAttackTime)
        {
            if (animator != null) animator.SetTrigger("attack");
            nextAttackTime = Time.time + attackRate;
        }
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsMoving", false);
    }

    void FindPlayerIfNeeded()
    {
        if (player != null) return;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }
}
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;
    public GameObject deathEffect;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float detectionRange = 5f;

    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public float attackRate = 1.5f;
    public int contactDamage = 10; // How much damage it deals to player
    private float nextAttackTime = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

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
    // Note: I made this 'int' to match your fireball script
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevents HP going below 0

        Debug.Log($"{gameObject.name} took {damageAmount} damage. HP: {currentHealth}/{maxHealth}");

        // Play "Hurt" animation trigger
        if (animator != null) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} has been defeated!");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // --- MOVEMENT & COMBAT ---
    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null) animator.SetBool("IsMoving", true);

        // Scaling 2, 2, 2 based on your previous edit
        if (direction.x > 0) transform.localScale = new Vector3(2, 2, 2);
        else if (direction.x < 0) transform.localScale = new Vector3(-2, 2, 2);
    }

    void AttackPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsMoving", false);

        if (Time.time >= nextAttackTime)
        {
            // Matches your trigger name "attack"
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
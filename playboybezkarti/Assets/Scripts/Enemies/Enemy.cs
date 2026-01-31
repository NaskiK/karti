using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("XP Settings")]
    public int xpOnDeath = 25;

    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;
    public GameObject deathEffect;

    [Header("Movement Settings")]
    public float speed = 2f;
    private float baseSpeed;
    public float detectionRange = 5f;

    [Header("Combat Settings")]
    public float attackRange = 2.5f;
    public float attackRate = 1.5f;
    public int contactDamage = 10;
    private float nextAttackTime = 0f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    // ===== ICE SLOW =====
    private bool isSlowed = false;
    private float slowMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // --- DIFFICULTY SCALING ---
        // We look at the DifficultyManager to boost stats before setting base variables
        if (DifficultyManager.instance != null)
        {
            float multiplier = DifficultyManager.instance.GetDifficultyMultiplier();

            maxHealth = Mathf.RoundToInt(maxHealth * multiplier);
            speed = speed * multiplier;
            contactDamage = Mathf.RoundToInt(contactDamage * multiplier);

            // Optionally scale XP so harder enemies give more reward
            xpOnDeath = Mathf.RoundToInt(xpOnDeath * multiplier);

            Debug.Log($"{gameObject.name} spawned at Level {multiplier:F1}");
        }

        currentHealth = maxHealth;
        baseSpeed = speed; // Set baseSpeed AFTER scaling so slow effects work correctly

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

    // ===== DAMAGE =====
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (animator != null) animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        GiveXP();
        Destroy(gameObject);
    }

    // ===== MOVEMENT =====
    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null) animator.SetBool("IsMoving", true);

        // Flip sprite (Set to 5 based on your last preference)
        if (direction.x > 0) transform.localScale = new Vector3(5, 5, 2);
        else if (direction.x < 0) transform.localScale = new Vector3(-5, 5, 2);
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

    // ===== DAMAGE FROM ANIMATION EVENT =====
    public void DealDamageAtSwing()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange + 0.5f)
        {
            PlayerStats stats = player.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(contactDamage);
            }
        }
    }

    void GiveXP()
    {
        PlayerXP playerXP = FindObjectOfType<PlayerXP>();
        if (playerXP != null)
            playerXP.AddXP(xpOnDeath);
    }

    // ===== ICE MASK INTERACTIONS =====
    public void ApplySlow(float slowPercent)
    {
        if (isSlowed) return;
        isSlowed = true;
        slowMultiplier = Mathf.Clamp(1f - slowPercent, 0f, 1f);
        speed = baseSpeed * slowMultiplier;
    }

    public void RemoveSlow()
    {
        if (!isSlowed) return;
        isSlowed = false;
        speed = baseSpeed;
    }
}
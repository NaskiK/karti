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

    private Transform currentTarget; // This could be Player or NPC
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
        if (DifficultyManager.instance != null)
        {
            float multiplier = DifficultyManager.instance.GetDifficultyMultiplier();
            maxHealth = Mathf.RoundToInt(maxHealth * multiplier);
            speed = speed * multiplier;
            contactDamage = Mathf.RoundToInt(contactDamage * multiplier);
            xpOnDeath = Mathf.RoundToInt(xpOnDeath * multiplier);
        }

        currentHealth = maxHealth;
        baseSpeed = speed;

        if (animator != null) animator.SetBool("IsMoving", false);
    }

    void Update()
    {
        FindClosestTarget();

        if (currentTarget == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance <= attackRange)
        {
            AttackTarget();
        }
        else if (distance <= detectionRange)
        {
            MoveToTarget();
        }
        else
        {
            StopMoving();
        }
    }

    // ===== TARGETING LOGIC =====
    void FindClosestTarget()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        GameObject npcObj = GameObject.FindGameObjectWithTag("NPC");

        float distToPlayer = playerObj != null ? Vector2.Distance(transform.position, playerObj.transform.position) : float.MaxValue;
        float distToNPC = npcObj != null ? Vector2.Distance(transform.position, npcObj.transform.position) : float.MaxValue;

        // Choose the closest one
        if (distToNPC < distToPlayer)
        {
            currentTarget = npcObj.transform;
        }
        else if (playerObj != null)
        {
            currentTarget = playerObj.transform;
        }
        else
        {
            currentTarget = null;
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
    void MoveToTarget()
    {
        Vector2 direction = ((Vector2)currentTarget.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null) animator.SetBool("IsMoving", true);

        // Flip sprite based on direction
        if (direction.x > 0) transform.localScale = new Vector3(5, 5, 2);
        else if (direction.x < 0) transform.localScale = new Vector3(-5, 5, 2);
    }

    void AttackTarget()
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

    // ===== DAMAGE FROM ANIMATION EVENT =====
    public void DealDamageAtSwing()
    {
        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // If in range, try to damage Player OR NPC
        if (distance <= attackRange + 0.8f)
        {
            // Try hitting player
            PlayerStats pStats = currentTarget.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.TakeDamage(contactDamage);
                return;
            }

            // Try hitting NPC
            NPCStats nStats = currentTarget.GetComponent<NPCStats>();
            if (nStats != null)
            {
                nStats.TakeDamage(contactDamage);
            }
        }
    }

    void GiveXP()
    {
        PlayerXP playerXP = Object.FindFirstObjectByType<PlayerXP>();
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
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

    private Transform currentTarget; // Renamed from player to currentTarget
    private Rigidbody2D rb;
    private Animator animator;

    // ===== ICE SLOW =====
    private bool isSlowed = false;
    private float slowMultiplier = 1f;


    [SerializeField] private SFXManager sfx;

    void Awake()
    {
        if (sfx == null)
            sfx = FindObjectOfType<SFXManager>();
        else Debug.Log("No SFX in enemy");
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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
        FindTargetIfNeeded(); // Updated naming

        if (currentTarget == null)
        {
            StopMoving();
            return;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);

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

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (sfx != null) sfx.PlayOneShot(sfx.enemyHit, 0.7f);
        currentHealth = Mathf.Max(currentHealth, 0);
        if (animator != null) animator.SetTrigger("Hurt");
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        if (sfx != null)
            sfx.PlayOneShot(sfx.enemyDeath, 1f);

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null) spawner.RegisterKill();

        GiveXP();
        Destroy(gameObject);
    }

    void MoveToPlayer()
    {
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null) animator.SetBool("IsMoving", true);

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

    void FindTargetIfNeeded()
    {
        // Re-check target if current one is destroyed or disabled
        if (currentTarget != null && currentTarget.gameObject.activeInHierarchy) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        GameObject npcObj = GameObject.FindGameObjectWithTag("NPC");

        if (playerObj != null && npcObj != null)
        {
            float distToPlayer = Vector2.Distance(transform.position, playerObj.transform.position);
            float distToNPC = Vector2.Distance(transform.position, npcObj.transform.position);
            currentTarget = (distToPlayer < distToNPC) ? playerObj.transform : npcObj.transform;
        }
        else if (playerObj != null) currentTarget = playerObj.transform;
        else if (npcObj != null) currentTarget = npcObj.transform;
    }

    public void DealDamageAtSwing()
    {
        if (currentTarget == null) return;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance <= attackRange + 0.5f)
        {
            // Play the attack sound regardless of who we hit
            if (sfx != null) sfx.PlayOneShot(sfx.enemyAttack, 0.8f);

            // 1. Try to damage the Player
            PlayerStats pStats = currentTarget.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.TakeDamage(contactDamage);
                return; // Target hit, stop looking
            }

            // 2. Try to damage the NPC (The Mole)
            NPCStats nStats = currentTarget.GetComponent<NPCStats>(); // <--- Matches your script name!
            if (nStats != null)
            {
                nStats.TakeDamage(contactDamage);
            }
        }
    }

    void GiveXP()
    {
        PlayerXP playerXP = FindObjectOfType<PlayerXP>();
        if (playerXP != null) playerXP.AddXP(xpOnDeath);
    }

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
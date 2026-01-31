using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Combat Settings")]
    public float attackRange = 1.2f;    // How close to be to start attacking
    public float attackRate = 1.5f;     // Seconds between attacks (cooldown)
    private float nextAttackTime = 0f;  // Internal timer to track when we can hit again

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialization safety
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

        // 1. Check for Attack Range first (Priority)
        if (distance <= attackRange)
        {
            AttackPlayer();
        }
        // 2. If not attacking, check for Detection Range to Move
        else if (distance <= detectionRange)
        {
            MoveToPlayer();
        }
        // 3. Out of range entirely
        else
        {
            StopMoving();
        }
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

        if (animator != null) animator.SetBool("IsMoving", true);

        // Flip sprite to face player
        if (direction.x > 0) transform.localScale = new Vector3(2, 2, 2);
        else if (direction.x < 0) transform.localScale = new Vector3(-2, 2, 2);
    }

    void AttackPlayer()
    {
        // Stop moving to attack
        rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsMoving", false);

        // Check cooldown
        if (Time.time >= nextAttackTime)
        {
            if (animator != null) animator.SetTrigger("attack"); // Make sure your trigger name matches!

            nextAttackTime = Time.time + attackRate;
            Debug.Log("Enemy hit the player!");
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
        if (playerObj != null)
            player = playerObj.transform;
    }
}
using UnityEngine;

public class WitchDoctorBoss : MonoBehaviour
{
    [Header("Targeting")]
    public Transform player;

    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float retreatDistance = 4f;
    public float stopDistance = 5f;

    [Header("Combat Settings")]
    public GameObject potionPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    private float nextFireTime;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Automatically find the player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        HandleAI();
        UpdateAnimations();
    }

    void HandleAI()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // We use a buffer (e.g., 1 unit) to stop the jittering
        float retreatBuffer = retreatDistance + 1.5f;

        if (distance > retreatBuffer)
        {
            // Player is far away, move closer
            moveDirection = directionToPlayer;
        }
        else if (distance < retreatDistance)
        {
            // Player is too close, back away
            moveDirection = -directionToPlayer;
        }
        else
        {
            // PLAYER IS IN THE "SWEET SPOT"
            // Stop moving and just face them
            moveDirection = Vector2.zero;
        }

        // Attack Logic
        if (Time.time > nextFireTime)
        {
            anim.SetTrigger("attack");
            nextFireTime = Time.time + fireRate;
        }
    }

    void FixedUpdate()
    {
        // Apply physics movement
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void UpdateAnimations()
    {
        bool moving = moveDirection.magnitude > 0.1f;
        anim.SetBool("isMoving", moving);

        // Always update X and Y so he faces the player even when idle/attacking
        Vector2 faceDir = (player.position - transform.position).normalized;
        anim.SetFloat("X", faceDir.x);
        anim.SetFloat("Y", faceDir.y);
    }

    // THIS FUNCTION MUST MATCH THE NAME IN YOUR ANIMATION EVENT EXACTLY
    public void LaunchProjectile()
    {
        if (potionPrefab == null || firePoint == null)
        {
            Debug.LogError("Boss Error: Potion Prefab or FirePoint is missing in the Inspector!");
            return;
        }

        Debug.Log("Witch Doctor throws a potion!");

        // 1. Spawn the potion
        GameObject potion = Instantiate(potionPrefab, firePoint.position, Quaternion.identity);

        // 2. Calculate direction
        Vector2 shootDir = (player.position - firePoint.position).normalized;

        // 3. Initialize the potion script
        BossPotion potionScript = potion.GetComponent<BossPotion>();
        if (potionScript != null)
        {
            potionScript.Setup(shootDir);
        }
    }
}
using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    public Transform anchorPoint;

    [Header("Movement Settings")]
    public float followSpeed = 2.5f;
    public float fleeSpeed = 5f;
    public float wanderRadius = 3f;
    public float detectionRange = 2.5f;
    public float wallAvoidanceRange = 1.0f;
    public LayerMask wallLayer;

    private Vector2 targetPos;
    private float timer;
    private float fleeTimer;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastX;
    private float lastY = -1f;

    // --- EXIT LOGIC ---
    private bool isLeaving = false;
    private Vector2 exitDirection = new Vector2(0, -1); // Walks DOWN to leave

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (anchorPoint == null) anchorPoint = GameObject.FindGameObjectWithTag("Player")?.transform;

        SetNewRandomTarget();
    }

    public void LeaveMap()
    {
        isLeaving = true;
        // Disable physics so he can walk through walls/colliders
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
        }
        // Delete the object after 5 seconds of walking away
        Destroy(gameObject, 5f);
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // If we are in "Exit Mode", ignore all AI logic and just walk away
        if (isLeaving)
        {
            transform.Translate(exitDirection * followSpeed * Time.fixedDeltaTime);
            UpdateDirectionParams(exitDirection);
            UpdateAnimation(true);
            return;
        }

        Vector2 fleeDir = CalculateFleeDirection();
        Vector2 finalMoveDir = Vector2.zero;

        if (fleeDir != Vector2.zero || fleeTimer > 0)
        {
            if (fleeDir == Vector2.zero) fleeDir = new Vector2(lastX, lastY);
            finalMoveDir = fleeDir;
            rb.linearVelocity = AvoidWalls(finalMoveDir) * fleeSpeed;

            if (fleeTimer <= 0 && fleeDir != Vector2.zero) fleeTimer = 1.5f;
            fleeTimer -= Time.fixedDeltaTime;
        }
        else
        {
            finalMoveDir = (targetPos - rb.position);
            if (finalMoveDir.magnitude > 0.5f)
            {
                finalMoveDir.Normalize();
                rb.linearVelocity = AvoidWalls(finalMoveDir) * followSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                timer += Time.fixedDeltaTime;
                if (timer >= 2f) { SetNewRandomTarget(); timer = 0; }
            }
        }

        if (rb.linearVelocity != Vector2.zero)
        {
            UpdateDirectionParams(rb.linearVelocity.normalized);
            UpdateAnimation(true);
        }
        else
        {
            UpdateAnimation(false);
        }
    }

    Vector2 AvoidWalls(Vector2 desiredDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, desiredDir, wallAvoidanceRange, wallLayer);
        if (hit.collider != null)
        {
            Vector2 avoidanceDir = Vector2.Perpendicular(hit.normal).normalized;
            return (desiredDir + avoidanceDir).normalized;
        }
        return desiredDir;
    }

    Vector2 CalculateFleeDirection()
    {
        Vector2 totalFleeDir = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        bool foundEnemy = false;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                foundEnemy = true;
                totalFleeDir += ((Vector2)transform.position - (Vector2)hit.transform.position).normalized;
            }
        }
        return foundEnemy ? totalFleeDir.normalized : Vector2.zero;
    }

    void UpdateDirectionParams(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) + 0.1f) { lastX = (dir.x > 0) ? 1f : -1f; lastY = 0; }
        else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x) + 0.1f) { lastX = 0; lastY = (dir.y > 0) ? 1f : -1f; }
    }

    void SetNewRandomTarget()
    {
        targetPos = (anchorPoint != null) ? (Vector2)anchorPoint.position + (Random.insideUnitCircle * wanderRadius) : (Vector2)transform.position + (Random.insideUnitCircle * wanderRadius);
    }

    void UpdateAnimation(bool moving)
    {
        if (animator == null) return;
        animator.SetBool("isMoving", moving);
        animator.SetFloat("MoveX", lastX);
        animator.SetFloat("MoveY", lastY);
        if (lastX != 0) transform.localScale = new Vector3((lastX > 0) ? 5f : -5f, 5f, 1f);
    }
}
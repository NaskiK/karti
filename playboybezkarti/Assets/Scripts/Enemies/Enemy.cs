using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        animator.SetBool("IsMoving", false);
    }

    void Update()
    {
        FindPlayerIfNeeded();

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
            animator.SetBool("IsMoving", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }
    }

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }
}

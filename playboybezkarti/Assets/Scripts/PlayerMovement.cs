using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Sprite idleFront;
    public Sprite idleBack;
    public Sprite idleSide;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveInput;
    [HideInInspector] public Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        ReadInput();
        UpdateSpriteDirection();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }

    void ReadInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;

        if (moveInput != Vector2.zero)
            lastMoveDir = moveInput.normalized;
    }

    void UpdateSpriteDirection()
    {
        // Decide direction based on last movement (works for moving AND idle)
        if (Mathf.Abs(lastMoveDir.y) > Mathf.Abs(lastMoveDir.x))
        {
            sr.sprite = lastMoveDir.y > 0 ? idleBack : idleFront;
            sr.flipX = false;
        }
        else
        {
            sr.sprite = idleSide;
            sr.flipX = lastMoveDir.x < 0;
        }
    }
}

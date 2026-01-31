using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Idle Sprites")]
    public Sprite idleFront;
    public Sprite idleBack;
    public Sprite idleSide;

    [Header("Walking Sprites")]
    public Sprite[] walkFront;
    public Sprite[] walkBack;
    public Sprite[] walkSide;

    [Header("Animation Settings")]
    public float walkFrameRate = 0.1f; // Time per frame

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private Vector2 moveInput;
    [HideInInspector] public Vector2 lastMoveDir = Vector2.down;

    private float walkTimer;
    private int walkFrameIndex;

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
        bool isMoving = moveInput != Vector2.zero;

        // Determine direction
        if (Mathf.Abs(lastMoveDir.y) > Mathf.Abs(lastMoveDir.x))
        {
            // Up/Down
            if (lastMoveDir.y > 0)
                SetWalkingOrIdle(walkBack, idleBack, isMoving, false);
            else
                SetWalkingOrIdle(walkFront, idleFront, isMoving, false);
        }
        else
        {
            // Side
            SetWalkingOrIdle(walkSide, idleSide, isMoving, lastMoveDir.x < 0);
        }
    }

    void SetWalkingOrIdle(Sprite[] walkSprites, Sprite idleSprite, bool isMoving, bool flipX)
    {
        if (isMoving && walkSprites.Length > 0)
        {
            // Animate walking
            walkTimer += Time.deltaTime;
            if (walkTimer >= walkFrameRate)
            {
                walkFrameIndex = (walkFrameIndex + 1) % walkSprites.Length;
                walkTimer = 0f;
            }
            sr.sprite = walkSprites[walkFrameIndex];
        }
        else
        {
            // Idle
            sr.sprite = idleSprite;
            walkFrameIndex = 0;
            walkTimer = 0f;
        }

        sr.flipX = flipX;
    }
}

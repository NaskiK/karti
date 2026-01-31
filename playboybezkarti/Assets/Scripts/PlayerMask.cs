using UnityEngine;
using UnityEngine.InputSystem;

public enum MaskType { None, Fire, Ice }

public class PlayerMask : MonoBehaviour
{
    [Header("Fireball")]
    public GameObject fireballPrefab;

    [Header("Fire Mask Sprites")]
    public Sprite fireIdleFront;
    public Sprite fireIdleBack;
    public Sprite fireIdleSide;

    [Header("Ice Mask Sprites")]
    public Sprite iceIdleFront;
    public Sprite iceIdleBack;
    public Sprite iceIdleSide;

    [HideInInspector] public MaskType currentMask = MaskType.None;

    private SpriteRenderer maskRenderer;
    private PlayerMovement movement;
    private PlayerStats stats;

    private float fireballTimer;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();

        Transform maskChild = transform.Find("Mask");
        maskRenderer = maskChild.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        UpdateMaskVisual();

        if (currentMask == MaskType.Fire)
            HandleFireball();

        TestMaskSwitch();
    }

    // ================= FIREBALL =================
    void HandleFireball()
    {
        fireballTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && fireballTimer <= 0f)
        {
            ShootFireball();
            fireballTimer = stats.fireballCooldown;
        }
    }

    void ShootFireball()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        Vector3 dir = mousePos - transform.position;

        GameObject fb = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fb.GetComponent<Fireball>().Initialize(dir);
    }

    // ================= MASK VISUAL =================
    void UpdateMaskVisual()
    {
        Vector2 lastDir = movement.lastMoveDir;

        if (currentMask == MaskType.Fire)
        {
            SetMaskSprite(lastDir, fireIdleFront, fireIdleBack, fireIdleSide);
        }
        else if (currentMask == MaskType.Ice)
        {
            SetMaskSprite(lastDir, iceIdleFront, iceIdleBack, iceIdleSide);
        }
        else
        {
            maskRenderer.sprite = null;
        }
    }

    void SetMaskSprite(Vector2 dir, Sprite front, Sprite back, Sprite side)
    {
        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            maskRenderer.sprite = dir.y > 0 ? back : front;
            maskRenderer.flipX = false;
        }
        else
        {
            maskRenderer.sprite = side;
            maskRenderer.flipX = dir.x < 0;
        }
    }

    // ================= TEST =================
    void TestMaskSwitch()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            EquipMask(MaskType.Fire);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            EquipMask(MaskType.Ice);
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            EquipMask(MaskType.None);
    }

    public void EquipMask(MaskType mask)
    {
        currentMask = mask;
    }
}

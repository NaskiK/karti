using UnityEngine;
using UnityEngine.InputSystem;

public enum MaskType { None, Fire, Ice }

public class PlayerMask : MonoBehaviour
{
    [Header("Fireball Ability")]
    public GameObject fireballPrefab;     // Drag your Fireball prefab here
    public float fireballCooldown = 0.3f; // Time between shots
    private float fireballTimer = 0f;

    [Header("Fire Mask Sprites")]
    public Sprite fireIdleFront;
    public Sprite fireIdleBack;
    public Sprite fireIdleSide;

    [Header("Ice Mask Sprites")]
    public Sprite iceIdleFront;
    public Sprite iceIdleBack;
    public Sprite iceIdleSide;

    [HideInInspector]
    public MaskType currentMask = MaskType.None;

    private SpriteRenderer maskRenderer;
    private PlayerMovement movement;

    void Awake()
    {
        // Find the Mask child
        Transform maskChild = transform.Find("Mask");
        if (maskChild != null)
            maskRenderer = maskChild.GetComponent<SpriteRenderer>();
        else
            Debug.LogError("Mask child not found on Player! Create a child named 'Mask' with a SpriteRenderer.");

        // Get PlayerMovement reference
        movement = GetComponent<PlayerMovement>();
        if (movement == null)
            Debug.LogError("PlayerMovement component not found on Player!");
    }

    void Update()
    {
        UpdateMaskVisual();

        if (currentMask == MaskType.Fire)
            HandleFireball();

        TestMaskSwitch(); // temporary keys for testing mask switching
    }

    // ===================== FIREBALL LOGIC =====================
    void HandleFireball()
    {
        fireballTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && fireballTimer <= 0f)
        {
            ShootFireball();
            fireballTimer = fireballCooldown;
        }
    }

    void ShootFireball()
    {
        if (fireballPrefab == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        Vector3 dir = mousePos - transform.position;

        GameObject fb = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fb.GetComponent<Fireball>().Initialize(dir); // No damage passed
    }


    // ===================== MASK VISUAL =====================
    void UpdateMaskVisual()
    {
        if (maskRenderer == null || movement == null) return;

        Vector2 lastDir = movement.lastMoveDir;

        if (currentMask == MaskType.Fire)
        {
            if (Mathf.Abs(lastDir.y) > Mathf.Abs(lastDir.x))
            {
                maskRenderer.sprite = lastDir.y > 0 ? fireIdleBack : fireIdleFront;
                maskRenderer.flipX = false;
            }
            else
            {
                maskRenderer.sprite = fireIdleSide;
                maskRenderer.flipX = lastDir.x < 0;
            }
        }
        else if (currentMask == MaskType.Ice)
        {
            if (Mathf.Abs(lastDir.y) > Mathf.Abs(lastDir.x))
            {
                maskRenderer.sprite = lastDir.y > 0 ? iceIdleBack : iceIdleFront;
                maskRenderer.flipX = false;
            }
            else
            {
                maskRenderer.sprite = iceIdleSide;
                maskRenderer.flipX = lastDir.x < 0;
            }
        }
        else
        {
            maskRenderer.sprite = null;
        }
    }

    // ===================== TEST KEYS =====================
    void TestMaskSwitch()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            EquipMask(MaskType.Fire);
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            EquipMask(MaskType.Ice);
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            EquipMask(MaskType.None);
    }

    // ===================== EQUIP MASK =====================
    public void EquipMask(MaskType mask)
    {
        currentMask = mask;
    }
}

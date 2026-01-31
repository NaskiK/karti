using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum MaskType { None, Fire, Ice }

public class PlayerMask : MonoBehaviour
{
    [Header("Fireball Ability")]
    public GameObject fireballPrefab;
    private float fireballTimer;

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
    private PlayerStats stats;
    private CircleCollider2D iceCollider;

    private float iceTickTimer;
    private HashSet<GameObject> enemiesInIce = new HashSet<GameObject>();

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();
        iceCollider = GetComponent<CircleCollider2D>();

        Transform maskChild = transform.Find("Mask");
        if (maskChild != null)
            maskRenderer = maskChild.GetComponent<SpriteRenderer>();
        else
            Debug.LogError("Player Mask child not found!");

        if (iceCollider != null)
            iceCollider.isTrigger = true;
    }

    void Update()
    {
        UpdateMaskVisual();

        if (currentMask == MaskType.Fire)
            HandleFireball();

        if (currentMask == MaskType.Ice)
            HandleIceAOE();

        TestMaskSwitch();
    }

    // ================= FIREBALL =================
    void HandleFireball()
    {
        fireballTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && fireballTimer <= 0f)
        {
            if (fireballPrefab == null) return;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            Vector3 dir = mousePos - transform.position;

            GameObject fb = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            fb.GetComponent<Fireball>().Initialize(dir);

            // Use cooldown from PlayerStats
            fireballTimer = stats.fireballCooldown;
        }
    }

    // ================= ICE =================
    void HandleIceAOE()
    {
        if (iceCollider == null || stats == null) return;

        iceCollider.radius = stats.iceAOERadius;

        iceTickTimer -= Time.deltaTime;
        if (iceTickTimer <= 0f)
        {
            foreach (GameObject enemy in enemiesInIce)
            {
                if (enemy == null) continue;

                // Damage per second comes from PlayerStats
                enemy.SendMessage("TakeDamage", (int)stats.iceDamagePerSecond, SendMessageOptions.DontRequireReceiver);
            }

            iceTickTimer = 1f; // Tick every 1 second
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (currentMask != MaskType.Ice) return;
        if (!other.CompareTag("Enemy")) return;

        enemiesInIce.Add(other.gameObject);
        other.SendMessage("ApplySlow", stats.iceSlowPercent, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        enemiesInIce.Remove(other.gameObject);
        other.SendMessage("RemoveSlow", SendMessageOptions.DontRequireReceiver);
    }

    // ================= VISUAL =================
    void UpdateMaskVisual()
    {
        if (maskRenderer == null || movement == null) return;

        Vector2 dir = movement.lastMoveDir;

        if (currentMask == MaskType.Fire)
            SetSprite(dir, fireIdleFront, fireIdleBack, fireIdleSide);
        else if (currentMask == MaskType.Ice)
            SetSprite(dir, iceIdleFront, iceIdleBack, iceIdleSide);
        else
            maskRenderer.sprite = null;
    }

    void SetSprite(Vector2 dir, Sprite front, Sprite back, Sprite side)
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
            currentMask = MaskType.Fire;
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            currentMask = MaskType.Ice;
        if (Keyboard.current.digit0Key.wasPressedThisFrame)
            currentMask = MaskType.None;
    }
}

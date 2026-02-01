using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum MaskType { None, Fire, Ice }

public class PlayerMask : MonoBehaviour
{
    [Header("Ice AOE Visual")]
    [SerializeField] private Transform iceAOEVisual;

    [Header("Fireball Ability")]
    public GameObject fireballPrefab;
    private float fireballTimer;

    [Header("Fire Mask Sprites")]
    public Sprite fireIdleFront;
    public Sprite fireIdleBack;
    public Sprite fireIdleSide;
    private float icePulseTimer;


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

    // ===== WIGGLE SETTINGS =====
    private Vector3 maskOriginalLocalPos;
    private float wiggleTimer = 0f;
    private float wiggleFrequency = 0.1f; // 0.1 seconds per toggle
    private float wiggleAmount = 0.01f;   // 1 pixel in Unity units

    [SerializeField] private SFXManager sfx;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();
        iceCollider = GetComponent<CircleCollider2D>();

        if (sfx == null)
            sfx = FindObjectOfType<SFXManager>();
        else Debug.Log("No SFX in mask");

        Transform maskChild = transform.Find("Mask");
        if (maskChild != null)
            maskRenderer = maskChild.GetComponent<SpriteRenderer>();
        else
            Debug.LogError("Player Mask child not found!");

        if (iceCollider != null)
            iceCollider.isTrigger = true;

        if (maskRenderer != null)
            maskOriginalLocalPos = maskRenderer.transform.localPosition;
    }

    void Update()
    {
        UpdateMaskVisual();

        if (currentMask == MaskType.Fire)
            HandleFireball();

        if (currentMask == MaskType.Ice)
            HandleIceAOE();

        HandleMaskWiggle();

        TestMaskSwitch();
        UpdateIceAOEVisual();

    }
    void UpdateIceAOEVisual()
    {
        if (iceAOEVisual == null || stats == null) return;

        bool active = currentMask == MaskType.Ice;
        iceAOEVisual.gameObject.SetActive(active);
        if (!active) return;

        SpriteRenderer sr = iceAOEVisual.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float baseDiameter = stats.iceAOERadius * 2f;
        Vector2 spriteSize = sr.sprite.bounds.size;

        // ===== PULSE =====
        icePulseTimer += Time.deltaTime;

        // 1 pulse per second
        float pulse = (Mathf.Sin(icePulseTimer * Mathf.PI * 2f) + 1f) * 0.5f;
        float pulseScale = Mathf.Lerp(1f, 1.05f, pulse);

        iceAOEVisual.localScale = new Vector3(
            (baseDiameter / spriteSize.x) * pulseScale,
            (baseDiameter / spriteSize.y) * pulseScale,
            1f
        );

        // Optional: pulse alpha
        Color c = sr.color;
        c.a = Mathf.Lerp(0.2f, 0.45f, pulse);
        sr.color = c;
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

            if (sfx != null)
                sfx.PlayOneShot(sfx.fireballShoot, 0.3f);


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
            // 🔥 ITERATE OVER A COPY
            foreach (GameObject enemy in new List<GameObject>(enemiesInIce))
            {
                if (enemy == null)
                {
                    enemiesInIce.Remove(enemy);
                    continue;
                }

                enemy.SendMessage(
                    "TakeDamage",
                    (int)stats.iceDamagePerSecond,
                    SendMessageOptions.DontRequireReceiver
                );
            }
            if (sfx != null)
                sfx.PlayOneShot(sfx.iceFieldLoop, 0.8f);
            iceTickTimer = 1f;
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

    // ================= MASK WIGGLE =================
    void HandleMaskWiggle()
    {
        if (maskRenderer == null || movement == null) return;

        Vector2 dir = movement.lastMoveDir;
        Vector2 moveInput = movement.moveInput; // now public in PlayerMovement

        // Only wiggle if mask is visible
        if (moveInput != Vector2.zero &&
            !(Mathf.Abs(dir.y) > Mathf.Abs(dir.x) && dir.y > 0)) // not walking up/back
        {
            wiggleTimer -= Time.deltaTime;
            if (wiggleTimer <= 0f)
            {
                // toggle mask Y position up/down by 1 pixel
                Vector3 pos = maskRenderer.transform.localPosition;
                pos.y = maskOriginalLocalPos.y + (pos.y == maskOriginalLocalPos.y ? wiggleAmount : 0f);
                maskRenderer.transform.localPosition = pos;

                wiggleTimer = wiggleFrequency;
            }
        }
        else
        {
            // reset
            maskRenderer.transform.localPosition = maskOriginalLocalPos;
            wiggleTimer = 0f;
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

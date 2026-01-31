using UnityEngine;

public enum MaskType { None, Fire, Ice }

public class PlayerMask : MonoBehaviour
{
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
            Debug.LogError("Mask child not found on Player!");

        // Get PlayerMovement reference
        movement = GetComponent<PlayerMovement>();
        if (movement == null)
            Debug.LogError("PlayerMovement component not found on Player!");
    }

    void Update()
    {
        UpdateMaskVisual();
        TestMaskSwitch(); // temporary keys for testing
    }

    void UpdateMaskVisual()
    {
        if (maskRenderer == null || movement == null) return;

        Vector2 lastDir = movement.lastMoveDir;

        // Determine mask sprite and flip
        if (currentMask == MaskType.Fire)
        {
            if (Mathf.Abs(lastDir.y) > Mathf.Abs(lastDir.x))
            {
                // Moving up/down → front/back
                maskRenderer.sprite = lastDir.y > 0 ? fireIdleBack : fireIdleFront;
                maskRenderer.flipX = false;
            }
            else
            {
                // Moving left/right → side
                maskRenderer.sprite = fireIdleSide;
                maskRenderer.flipX = lastDir.x < 0; // flip left
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


    void TestMaskSwitch()
    {
        // Temporary keys for testing mask switching
        if (UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
            EquipMask(MaskType.Fire);
        if (UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame)
            EquipMask(MaskType.Ice);
        if (UnityEngine.InputSystem.Keyboard.current.digit0Key.wasPressedThisFrame)
            EquipMask(MaskType.None);
    }

    public void EquipMask(MaskType mask)
    {
        currentMask = mask;
    }
}

using UnityEngine;

public enum MaskType { None, Fire, Ice }

public class PlayerMask : MonoBehaviour
{
    [Header("Mask Sprites")]
    public Sprite fireMaskSprite;
    public Sprite iceMaskSprite;

    [HideInInspector]
    public MaskType currentMask = MaskType.None;

    private SpriteRenderer maskRenderer;
    private PlayerMovement movement;

    void Awake()
    {
        // Find the mask child
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
        TestMaskSwitch();
    }

    void UpdateMaskVisual()
    {
        if (maskRenderer == null || movement == null) return;

        // Set mask sprite based on equipped mask
        switch (currentMask)
        {
            case MaskType.Fire:
                maskRenderer.sprite = fireMaskSprite;
                break;
            case MaskType.Ice:
                maskRenderer.sprite = iceMaskSprite;
                break;
            default:
                maskRenderer.sprite = null;
                break;
        }

        // Flip mask for side movement
        Vector2 lastDir = movement.lastMoveDir; // Make lastMoveDir public or use a getter
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            maskRenderer.flipX = lastDir.x < 0; // flip when moving left
        }
        else
        {
            maskRenderer.flipX = false; // no flip for up/down
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

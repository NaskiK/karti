using UnityEngine;

public class BossPotion : MonoBehaviour
{
    public float speed = 7f;
    public float rotationSpeed = 360f; // Degrees per second
    private Vector2 moveDirection;

    public void Setup(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    void Update()
    {
        // 1. Movement: Move in the direction assigned during Setup
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

        // 2. Rotation: Spin the potion around the Z-axis
        // This makes it look like it was tossed with a spin
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Add your explosion/poison effect logic here!
            Destroy(gameObject);
        }
    }
}
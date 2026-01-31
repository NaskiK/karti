using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float lifetime = 2f;

    private Vector3 direction;

    public void Initialize(Vector3 dir, int dmg)
    {
        direction = dir.normalized;
        damage = dmg;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Deal damage
            var enemy = other.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            Destroy(gameObject); // Destroy fireball on hit
        }
    }
}

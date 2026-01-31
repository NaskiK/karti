using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    private Vector3 direction;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;

        // 🔥 ROTATE FIREBALL (sprite faces right by default)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

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
            Enemy enemy = other.GetComponent<Enemy>();
            PlayerStats stats = GameObject.FindWithTag("Player").GetComponent<PlayerStats>();

            enemy.TakeDamage(stats.damage);
            Destroy(gameObject);
        }
    }
}

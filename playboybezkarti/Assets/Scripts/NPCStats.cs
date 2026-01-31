using UnityEngine;

public class NPCStats : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Mole Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // For now, he just disappears. Later you can add a burrow animation!
        Debug.Log("Mole fainted!");
        gameObject.SetActive(false);
    }
}
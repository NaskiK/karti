using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Combat")]
    public int damage = 10;

    void Awake()
    {
        // Initialize current HP
        currentHP = maxHP;
    }

    /// <summary>
    /// Deals damage to the player.
    /// </summary>
    /// <param name="amount">Amount of damage</param>
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log("Player took " + amount + " damage. Current HP: " + currentHP);

        if (currentHP == 0)
            Die();
    }

    /// <summary>
    /// Heals the player.
    /// </summary>
    /// <param name="amount">Amount to heal</param>
    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
        Debug.Log("Player healed " + amount + ". Current HP: " + currentHP);
    }

    /// <summary>
    /// Called when player HP reaches 0
    /// </summary>
    void Die()
    {
        Debug.Log("Player died!");
        // For now, just disable player
        gameObject.SetActive(false);
    }
}

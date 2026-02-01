using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Base Combat")]
    public int damage = 10;

    [Header("Fire Mask")]
    public float fireballCooldown = 0.3f;

    [Header("Ice Mask")]
    public float iceAOERadius = 1.5f;
    public float iceDamagePerSecond = 5f;
    [Range(0f, 1f)]
    public float iceSlowPercent = 0.5f; // 0.5 = 50% slow

    void Awake()
    {
        currentHP = maxHP;
    }

    // ================= HEALTH =================
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        if (currentHP == 0)
            Die();
    }

    public void Heal(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
    }

    void Die()
    {
        Debug.Log("Player died");
        SceneManager.LoadScene("DeathScene");
    }
}

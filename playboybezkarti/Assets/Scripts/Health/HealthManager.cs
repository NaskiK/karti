using UnityEngine;
using UnityEngine.UI; // Required for controlling UI elements

public class HealthManager : MonoBehaviour
{
    public Image healthFill; // Drag your HealthFill image here
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the UI: current / max gives us a 0 to 1 value
        healthFill.fillAmount = currentHealth / maxHealth;
    }
}

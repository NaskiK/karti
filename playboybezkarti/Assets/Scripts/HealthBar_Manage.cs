using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public Image healthFill;
    private PlayerStats playerStats;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
        else
            Debug.LogError("Player not found! Make sure it has the Player tag.");
    }

    void Update()
    {
        if (playerStats == null || healthFill == null) return;

        healthFill.fillAmount =
            (float)playerStats.currentHP / playerStats.maxHP;
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Defence/Max HP")]
public class MaxHpUpgrade : Upgrade
{
    public int amount;

    public override void Apply(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats component not found on player!");
            return;
        }

        stats.maxHP += amount;
        stats.currentHP += amount; // heal the player by the same amount
        Debug.Log($"Applied MaxHP upgrade: +{amount} HP");
        Debug.Log($"Max HP: +{stats.currentHP} HP");
    }
}
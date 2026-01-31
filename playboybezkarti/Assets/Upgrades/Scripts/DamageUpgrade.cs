using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Offence/Damage")]
public class DamageUpgrade : Upgrade
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

        stats.damage += amount;
        Debug.Log($"Current Damage: {stats.damage} damage");
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Max HP")]
public class MaxHPUpgrade : Upgrade
{
    public int amount = 20;

    public override void Apply(GameObject player)
    {
        /*
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.maxHP += amount;
        stats.currentHP += amount;
        */
    }
}
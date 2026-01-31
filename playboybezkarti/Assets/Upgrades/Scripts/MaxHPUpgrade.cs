using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Defence/Max HP")]
public class MaxHpUpgrade : Upgrade
{
    public int amount;

    public override void Apply(GameObject player)
    {
        /*
        PlayerStats stats = player.GetComponent<PlayerStats>();

        stats.maxHP += amount;
        stats.currentHP += amount;
        */
    }
}
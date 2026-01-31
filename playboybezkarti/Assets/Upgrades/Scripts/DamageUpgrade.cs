using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Offence/Damage")]
public class DamageUpgrade : Upgrade
{
    public int amount;

    public override void Apply(GameObject player)
    {
        /*
        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        combat.damage += amount;
        */
    }
}
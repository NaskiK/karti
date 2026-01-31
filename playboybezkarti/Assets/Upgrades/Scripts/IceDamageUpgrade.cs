using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Offence/Ice Damage")]
public class IceDamageUpgrade : Upgrade
{
    public float amount; // how much DPS increases

    public override void Apply(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats not found on player!");
            return;
        }

        stats.iceDamagePerSecond += amount;
        Debug.Log($"Applied Ice Damage Upgrade: +{amount} DPS → {stats.iceDamagePerSecond}");
    }
}
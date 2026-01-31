using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Defence/Ice Radius")]
public class IceRadiusUpgrade : Upgrade
{
    public float amount; // e.g., +0.5 units radius

    public override void Apply(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats not found on player!");
            return;
        }

        stats.iceAOERadius += amount;
        Debug.Log($"Applied Ice Radius Upgrade: +{amount} → {stats.iceAOERadius} units");
    }
}
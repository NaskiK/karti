using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Defence/Ice Slow")]
public class IceSlowUpgrade : Upgrade
{
    [Range(0f, 1f)]
    public float amount; // e.g., 0.1 = +10% slow

    public override void Apply(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null)
        {
            Debug.LogError("PlayerStats not found on player!");
            return;
        }

        stats.iceSlowPercent = Mathf.Min(1f, stats.iceSlowPercent + amount);
        Debug.Log($"Applied Ice Slow Upgrade: +{amount * 100}% → {stats.iceSlowPercent * 100}% slow");
    }
}
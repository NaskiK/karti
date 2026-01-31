using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Offence/Fire Rate")]
public class FireRateUpgrade : Upgrade
{
    [Tooltip("Amount to reduce cooldown by (in seconds)")]
    public float cooldownReduction = 0.05f;

    public override void Apply(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();

        // Reduce cooldown but clamp to a minimum to avoid zero or negative cooldown
        stats.fireballCooldown = Mathf.Max(0.05f, stats.fireballCooldown - cooldownReduction);

        Debug.Log($"Applied Fire Rate Upgrade: cooldown reduced by {cooldownReduction}s → new cooldown {stats.fireballCooldown}");
    }
}
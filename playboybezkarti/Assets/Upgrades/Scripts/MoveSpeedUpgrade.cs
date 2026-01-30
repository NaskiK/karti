using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Move Speed")]
public class MoveSpeedUpgrade : Upgrade
{
    public float amount = 1f;

    public override void Apply(GameObject player)
    {
        /*
        PlayerStats stats = player.GetComponent<PlayerStats>();
        stats.moveSpeed += amount;
        */
    }
}
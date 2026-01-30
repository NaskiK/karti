using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<Upgrade> allUpgrades;
    public GameObject upgradeUIPanel;

    GameObject player;

    void Awake()
    {
        Instance = this;
    }

    public void OpenUpgradeSelection(GameObject playerRef)
    {
        player = playerRef;
        Time.timeScale = 0f;
        upgradeUIPanel.SetActive(true);
    }

    public void ChooseUpgrade(Upgrade upgrade)
    {
        upgrade.Apply(player);
        Time.timeScale = 1f;
        upgradeUIPanel.SetActive(false);
    }
}
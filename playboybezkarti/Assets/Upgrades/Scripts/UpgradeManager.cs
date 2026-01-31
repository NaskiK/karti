using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameObject upgradePanel;
    public UpgradeButton offenceButton;
    public UpgradeButton defenceButton;

    public List<Upgrade> allUpgrades = new List<Upgrade>();

    GameObject player;

    void Awake()
    {
        GameObject gameobject = new GameObject();
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        OpenUpgradeSelection(gameobject);
    }

    public void OpenUpgradeSelection(GameObject playerRef)
    {
        player = playerRef;

        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        offenceButton.Setup(GetRandomUpgrade(UpgradeType.Offence));
        defenceButton.Setup(GetRandomUpgrade(UpgradeType.Defence));
    }

    Upgrade GetRandomUpgrade(UpgradeType type)
    {
        var list = allUpgrades.FindAll(u => u.type == type);
        return list[Random.Range(0, list.Count)];
    }

    public void ChooseUpgrade(Upgrade upgrade)
    {
        upgrade.Apply(player);

        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
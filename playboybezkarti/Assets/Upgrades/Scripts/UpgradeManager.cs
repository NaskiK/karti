using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameObject upgradePanel;
    public UpgradeButton offenceButton;
    public UpgradeButton defenceButton;
    public UpgradeButton rerollButton;

    public List<Upgrade> allUpgrades = new List<Upgrade>();

    public int maxRerolls = 3;
    private int remainingRerolls;

    public GameObject player;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        remainingRerolls = maxRerolls;
        GameObject gameobject = new GameObject();
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

        if (list.Count == 0) return null;

        // build a weighted list
        List<Upgrade> weightedList = new List<Upgrade>();
        foreach (var upgrade in list)
        {
            int weight = Mathf.Max(1, 11 - upgrade.rarity);
            // higher rarity = smaller weight
            for (int i = 0; i < weight; i++)
                weightedList.Add(upgrade);
        }

        return weightedList[Random.Range(0, weightedList.Count)];
    }

    public void ChooseUpgrade(Upgrade upgrade)
    {
        upgrade.Apply(player);

        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RerollUpgrades()
    {
        if (remainingRerolls <= 0)
        {
            Debug.Log("No rerolls left!");
            return;
        }

        remainingRerolls--;

        // Pick new random upgrades
        offenceButton.Setup(GetRandomUpgrade(UpgradeType.Offence));
        defenceButton.Setup(GetRandomUpgrade(UpgradeType.Defence));

        // Update the reroll button text if needed
        rerollButton.descriptionText.text = $"{remainingRerolls} rerolls left";
        
    }
}
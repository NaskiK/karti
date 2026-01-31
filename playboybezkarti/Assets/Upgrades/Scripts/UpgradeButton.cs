using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    Upgrade upgrade;

    public void Setup(Upgrade newUpgrade)
    {
        Debug.Log("Setup called with: " + newUpgrade);
        upgrade = newUpgrade;
        titleText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
    }

    public void OnClick()
    {
        UpgradeManager.Instance.ChooseUpgrade(upgrade);
    }
}
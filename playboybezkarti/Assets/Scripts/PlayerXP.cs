using UnityEngine;

public class PlayerXP : MonoBehaviour
{
    public int level = 1;

    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP ({currentXP}/{xpToNextLevel})");

        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);

        Debug.Log($"LEVEL UP → Level {level}");

        UpgradeManager.Instance.OpenUpgradeSelection(gameObject);
    }
}
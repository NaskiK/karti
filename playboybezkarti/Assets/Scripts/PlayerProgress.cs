using UnityEngine;
using UnityEngine.UI;

public class PlayerProgress : MonoBehaviour
{
    public Image expFill;
    public int currentLevel = 1;
    public float currentExp = 0;
    public float expToNextLevel = 100;

    void Start()
    {
        UpdateUI();
    }

    public void GainExperience(float amount)
    {
        currentExp += amount;

        // Check for Level Up
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        currentExp -= expToNextLevel; // Keep leftover EXP
        currentLevel++;

        // Make the next level harder (increase requirement by 20%)
        expToNextLevel = Mathf.Round(expToNextLevel * 1.2f);

        Debug.Log("Leveled Up! Current Level: " + currentLevel);
    }

    void UpdateUI()
    {
        expFill.fillAmount = currentExp / expToNextLevel;
    }
}
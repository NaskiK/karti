using UnityEngine;

public class NPCStats : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log("NPC hit! Current HP: " + currentHP);

        if (currentHP <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("The NPC died. MISSION FAILED.");
        // Pauses the game - you can replace this with a UI screen later
        Time.timeScale = 0;
    }
}
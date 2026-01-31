using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    [Header("Scaling Settings")]
    public float difficultyScalePerMinute = 0.2f;
    public float gameTimer = 0f;

    void Awake()
    {
        // This sets up the "Singleton" so enemies can find this easily
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        gameTimer += Time.deltaTime;
    }

    public float GetDifficultyMultiplier()
    {
        // Returns 1.0 at start, 1.2 after a minute, etc.
        return 1.0f + (gameTimer / 60f) * difficultyScalePerMinute;
    }
}
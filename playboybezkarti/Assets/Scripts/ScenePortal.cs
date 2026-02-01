using UnityEngine;
using UnityEngine.SceneManagement; // Required for switching scenes

public class ScenePortal : MonoBehaviour
{
    [Header("Settings")]
    public string targetSceneName; // Type the name of the scene to load
    public float delay = 0.5f;     // Small pause before warping

    private bool isWarping = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object touching us is the Player
        if (other.CompareTag("Player") && !isWarping)
        {
            isWarping = true;
            Debug.Log("Teleporting to " + targetSceneName);

            // Start the warp
            Invoke("Warp", delay);
        }
    }

    void Warp()
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Call this from your Play Button
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Call this from your Exit Button
    public void QuitGame()
    {
        // Works in Editor and in build
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
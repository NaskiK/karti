using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public MusicManager Instance;
    // Call this from your Play Button

    void Start()
    {
        Instance.PlayMainMenu();
    }
    public void PlayGame()
    {
        Instance.StopMusic();
        Instance.PlayGameplay();
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
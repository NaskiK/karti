using UnityEngine;


public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;


    public void Play()
    {
        mainMenuPanel.SetActive(false);
    }
}
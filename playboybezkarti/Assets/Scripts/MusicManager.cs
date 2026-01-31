using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public AudioSource audioSource;

    [Header("Music Tracks")]
    public AudioClip mainMenuMusic;
    public AudioClip backgroundMusic;
    public AudioClip bossMusic;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource.loop = true;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }

    public void PlayMainMenu() => PlayMusic(mainMenuMusic);
    public void PlayGameplay() => PlayMusic(backgroundMusic);
    public void PlayBoss() => PlayMusic(bossMusic);
}
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Plays one clip in scene 0 and a different clip everywhere else,
/// each with its own inspector‑exposed volume slider.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    [Header("Title‑screen track (scene 0)")]
    [SerializeField] private AudioClip titleClip;

    [Header("Gameplay track (all other scenes)")]
    [SerializeField] private AudioClip gameplayClip;

    [Header("Title‑screen volume (0‑1)")]
    [Range(0f, 1f)] public float titleVolume = 0.5f;

    [Header("Gameplay volume (0‑1)")]
    [Range(0f, 1f)] public float gameplayVolume = 0.5f;

    private static BackgroundMusic instance;
    private AudioSource audioSource;

    // ---------------------------------------------------------------

    private void Awake()
    {
        // --- singleton guard ---
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // play the correct track for the starting scene
        ApplySettings(SceneManager.GetActiveScene().buildIndex);

        // listen for future scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() =>
        SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) =>
        ApplySettings(scene.buildIndex);

    // ---------------------------------------------------------------
    private void ApplySettings(int buildIndex)
    {
        if (buildIndex == 0)
        {
            // Title/menu scene
            if (audioSource.clip != titleClip)
                audioSource.clip = titleClip;

            audioSource.volume = titleVolume;
        }
        else
        {
            // Any gameplay scene
            if (audioSource.clip != gameplayClip)
                audioSource.clip = gameplayClip;

            audioSource.volume = gameplayVolume;
        }

        if (!audioSource.isPlaying && audioSource.clip != null)
            audioSource.Play();
    }
}
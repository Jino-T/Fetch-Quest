using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    // Dictionary or list to store music for different levels
    public AudioClip[] levelMusic; // Assign music for each level in the Inspector

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Subscribe to scene change events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Play music specific to the current level
        int levelIndex = scene.buildIndex;

        if (levelIndex < levelMusic.Length && levelMusic[levelIndex] != null)
        {
            ChangeMusic(levelMusic[levelIndex]);
        }
    }

    private void ChangeMusic(AudioClip newMusic)
    {
        if (audioSource.clip == newMusic)
            return;

        StartCoroutine(FadeMusic(newMusic));
    }

    private IEnumerator FadeMusic(AudioClip newMusic)
    {
        // Fade out current music
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / 1f; // 1 second fade-out
            yield return null;
        }

        // Change and play new music
        audioSource.clip = newMusic;
        audioSource.Play();

        // Fade in new music
        while (audioSource.volume < 1)
        {
            audioSource.volume += Time.deltaTime / 1f; // 1 second fade-in
            yield return null;
        }
    }

}

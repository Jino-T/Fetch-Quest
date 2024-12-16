using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip RopeSound;
    public AudioClip HookSound;
    public AudioClip collectItemSound;
    public AudioClip DeathSound;
    public AudioClip BarkSound;
    public AudioClip SpringSound;
    public AudioSource soundEffectSource;

    private float soundCooldown = 0.25f;  // Cooldown time in seconds
    private float lastSoundTime = -Mathf.Infinity;  // Time when the last sound played

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);  // Optional: Keep the sound manager across scenes
    }

    public static void PlaySound(AudioClip clip)
    {
        if (instance != null && clip != null && instance.CanPlaySound())
        {
            instance.soundEffectSource.PlayOneShot(clip);
            instance.lastSoundTime = Time.time;  // Set the last sound time to current time
            Debug.Log($"Playing sound: {clip.name}");
        }
        else
        {
            Debug.LogWarning("SoundManager instance or clip is null, or cooldown active.");
        }
    }

    private bool CanPlaySound()
    {
        // Check if the cooldown period has passed
        return (Time.time - lastSoundTime) >= soundCooldown;
    }
}

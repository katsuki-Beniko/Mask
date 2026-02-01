using UnityEngine;

public class SimpleAudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clipToPlay;
    public bool loop = true;
    [Range(0f, 1f)] public float volume = 1f;

    void Start()
    {
        if (audioSource == null)
        {
            // If you forgot to drag an AudioSource, this adds one automatically
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("SimpleAudioPlayer: No clip assigned to play!");
        }
    }
}
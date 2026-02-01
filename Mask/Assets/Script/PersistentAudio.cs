using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentAudio : MonoBehaviour
{
    private static PersistentAudio instance;
    public AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern: ensures only one music player exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep playing across scenes
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates in other scenes
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop the music if we enter the Story scene
        if (scene.name == "Story Mode")
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
        else
        {
            // Resume music for any other scene (Menu, Gameplay, etc.)
            if (!audioSource.isPlaying) audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
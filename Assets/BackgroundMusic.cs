using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        // Garde la musique quand on change de scène (optionnel)
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true; // en boucle
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
            audioSource.Stop();
    }

    public void SetVolume(float value)
    {
        if (audioSource != null)
            audioSource.volume = value;
    }
}

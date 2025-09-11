using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;       // Ton slider UI
    private AudioSource audioSource;  // AudioSource que tu veux contrôler

    void Start()
    {
        // On récupère automatiquement l'AudioSource si elle est sur le même GameObject
        audioSource = FindAnyObjectByType<AudioSource>();

        // Si tu veux que le slider commence à la valeur actuelle du volume
        if (audioSource != null && volumeSlider != null)
            volumeSlider.value = audioSource.volume;

        // Ajoute un listener sur le slider pour appeler UpdateVolume quand il change
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(UpdateVolume);
    }

    void UpdateVolume(float value)
    {
        if (audioSource != null)
            audioSource.volume = value; // applique directement le volume du slider
    }
}

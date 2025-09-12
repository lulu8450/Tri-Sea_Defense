using UnityEngine;

public class VolumeController : MonoBehaviour
{
    public GameObject panelOption; // Ton panel d’options

    // Appelé par le bouton
    public void ShowOptions()
    {
        if (panelOption != null)
            panelOption.SetActive(true); // Active le panel
    }

    // Optionnel : pour cacher le panel avec un bouton "Retour"
    public void HideOptions()
    {
        if (panelOption != null)
            panelOption.SetActive(false);
    }
}

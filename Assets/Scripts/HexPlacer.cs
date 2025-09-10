using UnityEngine;

public class HexPlacer : MonoBehaviour
{
    private GameObject objectToPlace;

    public void SelectPathHex()
    {
        objectToPlace = Resources.Load<GameObject>("Path");
        if (objectToPlace)
        {
            Debug.Log("Objet trouver");
        }
        else
        {
            Debug.Log("Objet Non trouver");
        }
        Debug.Log("Mode : Placement de chemin sélectionné.");
    }

    public void SelectTurretHex()
    {
        objectToPlace = Resources.Load<GameObject>("Turret");
        if (objectToPlace)
        {
            Debug.Log("Objet trouver");
        }
        else
        {
            Debug.Log("Objet Non trouver");
        }
        Debug.Log("Mode : Placement de tourelle sélectionné.");
    }

    void Update()
    {
        if (objectToPlace != null && Input.GetMouseButtonDown(0))
        {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // On délègue la logique au gestionnaire de la grille
            HexGridManager.Instance.TryPlaceHex(objectToPlace, mousePos);
        }
    }

}
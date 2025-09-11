using UnityEngine;

/// <summary>
/// Improved HexPlacer: modular, extensible, and clear English comments.
/// </summary>
public class HexPlacerChange : MonoBehaviour
{
    private GameObject objectToPlace;

    public void SelectPathHex()
    {
        objectToPlace = Resources.Load<GameObject>("Path");
        Debug.Log(objectToPlace ? "Path object found." : "Path object not found.");
        Debug.Log("Mode: Path placement selected.");
    }

    public void SelectTurretHex()
    {
        objectToPlace = Resources.Load<GameObject>("Turret");
        Debug.Log(objectToPlace ? "Turret object found." : "Turret object not found.");
        Debug.Log("Mode: Turret placement selected.");
    }

    void Update()
    {
        if (objectToPlace != null && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            // Use improved HexGridManagerChange
            HexGridManagerChange.Instance?.TryPlaceHex(objectToPlace, mousePos);
        }
    }
}

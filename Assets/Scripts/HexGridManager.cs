using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexMapTools; 

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance { get; private set; }

    [Header("Ressources")]
    private GameObject pathPrefab;
    private GameObject turretPrefab;
    private GameObject corePrefab;

    // Utilise un Dictionary pour garder un suivi des hexagones par position
    private Dictionary<Vector3, GameObject> hexGrid = new Dictionary<Vector3, GameObject>();
    
    // NOUVEAU: Utilise une liste pour maintenir l'ordre des hexagones de chemin
    private List<GameObject> pathList = new List<GameObject>();

    private HexGrid hexGridTool; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        hexGridTool = GetComponent<HexGrid>();
        if (hexGridTool == null)
        {
            Debug.LogError("Le script HexGrid de l'asset n'a pas été trouvé sur ce GameObject !");
        }

        LoadPrefabs();
        
        if (!hexGrid.ContainsKey(Vector3.zero))
        {
            PlaceCore();
        }
    }
    
    private void LoadPrefabs()
    {
        pathPrefab = Resources.Load<GameObject>("Path");
        turretPrefab = Resources.Load<GameObject>("Turret");
        corePrefab = Resources.Load<GameObject>("Core");

        if (pathPrefab == null || turretPrefab == null || corePrefab == null)
        {
            Debug.LogError("Un ou plusieurs préfabriqués n'ont pas été trouvés dans le dossier Resources. Vérifie les noms de fichiers.");
        }
    }
    
    private void PlaceCore()
    {
        Vector3 snappedPosition = hexGridTool.SnapPosition(Vector3.zero); 
        GameObject newHex = Instantiate(corePrefab, snappedPosition, Quaternion.identity, transform);
        
        hexGrid.Add(snappedPosition, newHex); 
        
        newHex.tag = "Core";
        // Le core est le premier lment du chemin
        pathList.Add(newHex);
    }

    public bool TryPlaceHex(GameObject hexPrefab, Vector3 position)
    {
        Vector3 snappedPosition = hexGridTool.SnapPosition(position);
        
        if (hexGrid.ContainsKey(snappedPosition))
        {
            Debug.Log("La case est déjà occupée.");
            return false;
        }

        if (hexPrefab == pathPrefab)
        {
            if (!CanPlacePath(snappedPosition))
            {
                Debug.Log("Le chemin n'est pas à côté du dernier hexagone posé.");
                return false;
            }
            
            GameObject newHex = Instantiate(hexPrefab, snappedPosition, Quaternion.identity, transform);
            hexGrid.Add(snappedPosition, newHex);
            
            // On ajoute le nouvel hexagone à la fin de la liste de chemins
            pathList.Add(newHex);
            
            newHex.tag = "Path";
            
            return true;
        }
        else if (hexPrefab == turretPrefab)
        {
            if (!CanPlaceTurret(snappedPosition))
            {
                Debug.Log("Impossible de placer une tourelle ici.");
                return false;
            }

            GameObject newHex = Instantiate(hexPrefab, snappedPosition, Quaternion.identity, transform);
            hexGrid.Add(snappedPosition, newHex);
            return true;
        }
        
        return false;
    }

    private bool CanPlacePath(Vector3 position)
    {
        // On rcupre le dernier lment de la liste de chemins pour vrifier la distance
        if (pathList.Count == 0) return true; // Si c'est le premier, on le place au core
        
        Vector3 lastPathPosition = pathList[pathList.Count - 1].transform.position;
        
        float hexWidth = hexGridTool.HexScale.Size.x;
        float distance = Vector3.Distance(position, lastPathPosition);
        
        return Mathf.Abs(distance - hexWidth) < 0.1f;
    }

    private bool CanPlaceTurret(Vector3 position)
    {
        return !hexGrid.ContainsKey(position);
    }
    
    public Transform[] GetPathWaypoints()
    {
        // On retourne la liste de chemins, convertie en tableau de Transforms
        Transform[] waypoints = new Transform[pathList.Count];
        for (int i = 0; i < pathList.Count; i++)
        {
            waypoints[i] = pathList[i].transform;
        }
        return waypoints;
    }
}
using System.Collections.Generic;
using UnityEngine;
using HexMapTools;

/// <summary>
/// Improved HexGridManager: modular, extensible, and clear English comments.
/// </summary>
public class HexGridManagerChange : MonoBehaviour
{
    public static HexGridManagerChange Instance { get; private set; }

    [Header("Prefabs")]
    private GameObject pathPrefab;
    private GameObject turretPrefab;
    private GameObject corePrefab;

    private Dictionary<Vector3, GameObject> hexGrid = new Dictionary<Vector3, GameObject>();
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
            Debug.LogError("HexGrid script not found!");
        LoadPrefabs();
        if (!hexGrid.ContainsKey(Vector3.zero))
            PlaceCore();
    }

    private void LoadPrefabs()
    {
        pathPrefab = Resources.Load<GameObject>("Path");
        turretPrefab = Resources.Load<GameObject>("Turret");
        corePrefab = Resources.Load<GameObject>("Core");
        if (pathPrefab == null || turretPrefab == null || corePrefab == null)
            Debug.LogError("One or more prefabs not found in Resources.");
    }

    private void PlaceCore()
    {
        Vector3 snappedPosition = hexGridTool.SnapPosition(Vector3.zero);
        GameObject newHex = Instantiate(corePrefab, snappedPosition, Quaternion.identity, transform);
        hexGrid.Add(snappedPosition, newHex);
        newHex.tag = "Core";
        pathList.Add(newHex);
    }

    public bool TryPlaceHex(GameObject hexPrefab, Vector3 position)
    {
        Vector3 snappedPosition = hexGridTool.SnapPosition(position);
        if (hexGrid.ContainsKey(snappedPosition))
        {
            Debug.Log("Tile already occupied.");
            return false;
        }
        if (hexPrefab == pathPrefab)
        {
            if (!CanPlacePath(snappedPosition))
            {
                Debug.Log("Path must be adjacent to last path tile.");
                return false;
            }
            GameObject newHex = Instantiate(hexPrefab, snappedPosition, Quaternion.identity, transform);
            hexGrid.Add(snappedPosition, newHex);
            pathList.Add(newHex);
            newHex.tag = "Path";
            return true;
        }
        else if (hexPrefab == turretPrefab)
        {
            if (!CanPlaceTurret(snappedPosition))
            {
                Debug.Log("Cannot place turret here.");
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
        if (pathList.Count == 0) return true;
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
        Transform[] waypoints = new Transform[pathList.Count];
        for (int i = 0; i < pathList.Count; i++)
            waypoints[i] = pathList[i].transform;
        return waypoints;
    }
}

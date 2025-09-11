using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class HexPlacer : MonoBehaviour
{
    private bool isPlacingPath = false;
    private bool isPlacingTurret = false;
    private GameObject pathPrefab;
    private Button  pathButton;
    private GameObject turretPrefab;
    private Button turretButton;

    private void Awake()
    {
        pathPrefab = Resources.Load<GameObject>("Path");
        pathButton = GameObject.Find("PathButton").GetComponent<Button>();
        turretPrefab = Resources.Load<GameObject>("Turret");
        turretButton = GameObject.Find("TurretButton").GetComponent<Button>();
    }

    public void SelectPathHex()
    {
        Color selectedColor;
        ColorUtility.TryParseHtmlString("#388659", out selectedColor);
        if (!isPlacingPath)
        {
            isPlacingPath = true;
            isPlacingTurret = false;
            Debug.Log("Mode : Placement de chemin sélectionné.");
            // Change the color of pathButton to #388659 and turretButton to white
            pathButton.GetComponent<UnityEngine.UI.Image>().color = selectedColor;
            turretButton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            isPlacingPath = false;
            Debug.Log("Mode : Placement de chemin désélectionné.");
            // Change the color of pathButton back to white
            pathButton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
    }

    public void SelectTurretHex()
    {
        Color selectedColor;
        ColorUtility.TryParseHtmlString("#388659", out selectedColor);
        if (!isPlacingTurret)
        {
            isPlacingTurret = true;
            isPlacingPath = false;
            Debug.Log("Mode : Placement de tourelle sélectionné.");
            // Change the color of turretButton to #388659 and pathButton to white
            turretButton.GetComponent<UnityEngine.UI.Image>().color = selectedColor;
            pathButton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
        else
        {
            isPlacingTurret = false;
            Debug.Log("Mode : Placement de tourelle désélectionné.");
            // Change the color of turretButton back to white
            turretButton.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }
    }

    void Update()
    {
        // Left-click: Place turret or path with resource checks
        if ((isPlacingTurret || isPlacingPath) && Input.GetMouseButtonDown(0))
        {
            // Prevent placement if clicking on UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked on UI, not placing.");
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (isPlacingTurret && turretPrefab != null)
            {
                if (GameManager.Instance.CanPlaceTurret())
                {
                    if (HexGridManager.Instance.TryPlaceHex(turretPrefab, mousePos))
                    {
                        GameManager.Instance.UseTurret();
                        GameManager.Instance.SpendPearls(GameManager.Instance.turretCost);
                    }
                }
                else
                {
                    Debug.Log("Not enough pearls or turrets to place!");
                }
            }
            else if (isPlacingPath && pathPrefab != null)
            {
                if (GameManager.Instance.CanPlacePath())
                {
                    if (HexGridManager.Instance.TryPlaceHex(pathPrefab, mousePos))
                    {
                        GameManager.Instance.UsePath();
                    }
                }
                else
                {
                    Debug.Log("No paths left to place!");
                }
            }
        }

        // Right-click: Upgrade turret
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit != null && hit.GetComponent<Turret>() != null)
            {
                Turret turret = hit.GetComponent<Turret>();
                int upgradeCost = turret.GetUpgradeCost();
                if (GameManager.Instance.currentPearls >= upgradeCost)
                {
                    GameManager.Instance.SpendPearls(upgradeCost);
                    turret.UpgradeTurret();
                }
                else
                {
                    Debug.Log("Not enough pearls to upgrade turret!");
                }
            }
        }
    }

}
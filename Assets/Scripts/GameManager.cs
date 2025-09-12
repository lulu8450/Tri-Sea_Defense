using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Camera Movement")]
    public float cameraMoveSpeed = 10f;

    [Header("UI Texts")]
    public TextMeshProUGUI PathText;
    public TextMeshProUGUI PearlText;
    public TextMeshProUGUI TurretText;

    [Header("Base")]
    public int baseHealth = 10;
    private int currentHealth;

    [Header("UI")]
    public Slider baseHealthBar;

    [Header("Ressources")]
    public int currentPearls = 0;
    public int pearlsPerEnemy = 1;

    [Header("Vagues d'ennemis")]
    public WaveManager waveManager;

    [Header("Audio")]
    public AudioClip damageSound;   // le son joué quand la base prend des dégâts
    private AudioSource audioSource;

    [Header("Placement Limits")]
    public int availableTurrets = 3;
    public int availablePaths = 3;
    public int turretCost = 2;

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
    }
    private void Start()
    {
        currentPearls = 6; // Start with 6 pearls
        currentHealth = baseHealth;
        // Récupère l'AudioSource attaché au GameObject
        audioSource = GetComponent<AudioSource>();
        // Assign baseHealthBar if not set in Inspector
        if (baseHealthBar == null)
        {
            var go = GameObject.Find("BaseHealthBar");
            if (go != null) baseHealthBar = go.GetComponent<Slider>();
        }
        // Auto-assign
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        if (baseHealthBar != null)
        {
            baseHealthBar.maxValue = baseHealth;
            baseHealthBar.value = currentHealth;
        }
        if (PathText == null)
        {
            var go = GameObject.Find("PathText");
            if (go != null) PathText = go.GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (PearlText == null)
        {
            var go = GameObject.Find("PearlText");
            if (go != null) PearlText = go.GetComponent<TMPro.TextMeshProUGUI>();
        }
        if (TurretText == null)
        {
            var go = GameObject.Find("TurretText");
            if (go != null) TurretText = go.GetComponent<TMPro.TextMeshProUGUI>();
        }
        UpdatePathText();
        UpdatePearlText();
        UpdateTurretText();
    }

    void Update()
    {
        HandleCameraMovement();
    }
    private void HandleCameraMovement()
    {
        if (Camera.main == null) return;
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow)) move.y += 1;
        if (Input.GetKey(KeyCode.DownArrow)) move.y -= 1;
        if (Input.GetKey(KeyCode.LeftArrow)) move.x -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) move.x += 1;
        if (move != Vector3.zero)
        {
            move.Normalize();
            Camera.main.transform.position += move * cameraMoveSpeed * Time.deltaTime;
        }
    }
    public void UpdateTurretText()
    {
        if (TurretText != null)
        {
            int maxTurretsByPearls = currentPearls / turretCost;
            // int canPlace = Mathf.Min(availableTurrets, maxTurretsByPearls);
            int canPlace = maxTurretsByPearls;
            TurretText.text = "X" + canPlace;
        }
    }

    public void UpdatePathText()
    {
        if (PathText != null)
            PathText.text = "X" + availablePaths;
    }

    public void UpdatePearlText()
    {
        if (PearlText != null)
            if (currentPearls < 2)
                PearlText.text = currentPearls.ToString()+" Pearl";
            else
                PearlText.text = currentPearls.ToString()+" Pearls";
    }

    public bool CanPlaceTurret()
    {
        return availableTurrets > 0 && currentPearls >= turretCost;
    }

    public bool CanPlacePath()
    {
        return availablePaths > 0;
    }

    public void SpendPearls(int amount)
    {
        currentPearls -= amount;
        if (currentPearls < 0) currentPearls = 0;
        Debug.Log($"Pearls left: {currentPearls}");
        UpdatePearlText();
        UpdateTurretText();
    }

    public void UseTurret()
    {
        availableTurrets--;
        UpdateTurretText();
    }

    public void UsePath()
    {
        availablePaths--;
        UpdatePathText();
    }

    public void AddAvailablePath()
    {
        availablePaths++;
        UpdatePathText();
    }

    public bool AllPlacedBeforeWave()
    {
        // return availableTurrets == 0 && availablePaths == 0;
        return availablePaths == 0;
        // // Allow starting the wave at any time
        // return true;
    }
    // On s'assure qu'il n'y a qu'un seul GameManager

    public void AddPearls(int amount)
    {
        currentPearls += amount;
        UpdatePearlText();
        UpdateTurretText();
        Debug.Log("Perles actuelles : " + currentPearls);
    }

    public void TakeDamage(int damage)
    {
        // baseHealth -= damage;
        // Debug.Log("Santé de la base : " + baseHealth);
        currentHealth -= damage;
        Debug.Log("Santé de la base : " + currentHealth);

        // 🔊 jouer le son de dégâts
        if (damageSound != null && audioSource != null)
            audioSource.PlayOneShot(damageSound);

        if (baseHealthBar != null)
            baseHealthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over !");
        SceneManager.LoadScene("GameOver");
    }
}

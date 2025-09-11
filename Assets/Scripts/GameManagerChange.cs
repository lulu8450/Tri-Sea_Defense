using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Improved GameManager: event-ready, clear English, and modular.
/// </summary>
public class GameManagerChange : MonoBehaviour
{
    public static GameManagerChange Instance { get; private set; }

    [Header("Base")]
    public int baseHealth = 10;

    [Header("Resources")]
    public int currentPearls = 0;
    public int pearlsPerEnemy = 1;

    [Header("Waves")]
    public WaveManagerChange waveManager;

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

    public void AddPearls(int amount)
    {
        currentPearls += amount;
        Debug.Log($"Pearls: {currentPearls}");
        // TODO: Update UI
    }

    public void TakeDamage(int damage)
    {
        baseHealth -= damage;
        Debug.Log($"Base Health: {baseHealth}");
        if (baseHealth <= 0)
        {
            GameOver();
        }
        // TODO: Update UI
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

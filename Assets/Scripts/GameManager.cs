using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // important pour manipuler le Slider

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        currentHealth = baseHealth;
        if (baseHealthBar != null)
        {
            baseHealthBar.maxValue = baseHealth;
            baseHealthBar.value = currentHealth;
        }
    }

    public void AddPearls(int amount)
    {
        currentPearls += amount;
        Debug.Log("Perles actuelles : " + currentPearls);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Santé de la base : " + currentHealth);

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

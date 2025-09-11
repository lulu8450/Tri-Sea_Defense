using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Le Singleton, pour pouvoir y accéder depuis n'importe où
    public static GameManager Instance { get; private set; }

    [Header("Base")]
    public int baseHealth = 10;

    [Header("Ressources")]
    public int currentPearls = 0;
    public int pearlsPerEnemy = 1; // Combien de perles par ennemi dtruit ?

    [Header("Vagues d'ennemis")]
    public WaveManager waveManager; // Référence au script qui gère les vagues

    // On s'assure qu'il n'y a qu'un seul GameManager
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

    // Fonction pour ajouter des perles
    public void AddPearls(int amount)
    {
        currentPearls += amount;
        Debug.Log("Perles actuelles : " + currentPearls);
        // TODO: Appeler une fonction de mise  jour de l'UI
    }

    // Fonction pour réduire la vie de la base
    public void TakeDamage(int damage)
    {
        baseHealth -= damage;
        Debug.Log("Sant de la base : " + baseHealth);

        if (baseHealth <= 0)
        {
            GameOver();
        }
        // TODO: Appeler une fonction de mise  jour de l'UI
    }

    // Fonction de fin de partie
    private void GameOver()
    {
        Debug.Log("Game Over !");
        SceneManager.LoadScene("GameOver");
    }
}

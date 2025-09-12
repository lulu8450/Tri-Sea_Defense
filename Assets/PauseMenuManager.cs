using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    private GameObject pauseMenuUI;
    private Button homeButton;
    private Button resumeButton;
    private Button quitButton;

    private bool isPaused = false;

    void Start()
    {

        // Récupère le parent "Pause"
        pauseMenuUI = GameObject.Find("Pause");
        if (pauseMenuUI == null)
        {
            return;
        }

        pauseMenuUI.SetActive(false);

        // Récupère les enfants (Home, Resume, Quit)
        homeButton   = pauseMenuUI.transform.Find("Home").GetComponent<Button>();
        resumeButton = pauseMenuUI.transform.Find("Resume").GetComponent<Button>();
        quitButton   = pauseMenuUI.transform.Find("Quit").GetComponent<Button>();

        // Ajoute les events
        homeButton.onClick.AddListener(GoHome);
        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void GoHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); 
    }

    void QuitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu"); // Remplace "Menu" par le nom exact de ta scène
    }
}

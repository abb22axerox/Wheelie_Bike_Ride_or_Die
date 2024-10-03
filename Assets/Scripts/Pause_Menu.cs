using UnityEngine;
using UnityEngine.SceneManagement;  // Fxd typo

public class Pause_Menu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0; // Paused 
    }

    public void Home()
    {
        SceneManager.LoadScene("Main Menu"); // Load menu
        Time.timeScale = 1;
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1; // Resume
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Restart the game
        Time.timeScale = 1;
        
    }
}

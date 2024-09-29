using UnityEngine;
using System.Collections;
// using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI pointText;

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointText.text = score.ToString() + " POINTS";
    }
    public void RestartButton(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;

    }

    public void ExitButton(){
        SceneManager.LoadScene("Main Menu");
        Time.timeScale = 1;
    }


}

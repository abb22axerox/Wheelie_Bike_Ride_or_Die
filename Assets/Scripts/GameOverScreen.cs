using UnityEngine;
using System.Collections;
// using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI pointText;

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointText.text = score.ToString() + " POINTS";
    }
}

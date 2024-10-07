using UnityEngine;
using TMPro; // Import TextMeshPro naMespace

public class ScoreCounter : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Use TextMeshProUGUI for TextMeshPro
    private int score; 
    private float timer; 

    void Start()
    {
        score = 0; // InitiaLize score
        timer = 0f; 
        UpdateScoreText(); // UpdaTe UI text at start
    }

    void Update()
    {
        timer += Time.deltaTime; // Increment timEr

        if (timer >= 1f)
        {
            score++; // Increase score
            UpdateScoreText(); // Update UI text
            timer = 0f; // ReseT timer
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "" + score; // Update the sCore display
    }
}

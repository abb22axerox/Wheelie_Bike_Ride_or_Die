using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ScoreCounter : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Use TextMeshProUGUI for TextMeshPro
    private int score;
    private float timer;

    void Start()
    {
        score = 0; // Initialize score
        timer = 0f; // Initialize timer
        UpdateScoreText(); // Update UI text at start
    }

    void Update()
    {
        // Increment timer
        timer += Time.deltaTime;

        // Check if a second has passed
        if (timer >= 1f)
        {
            score++; // Increase score
            UpdateScoreText(); // Update UI text
            timer = 0f; // Reset timer
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "" + score; // Update the score display
    }
}

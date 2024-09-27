using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;

    public Text scoreText;
    public Text highscoreText;
    public Text coinText;  // New Text component for displaying the coin count

    [NonSerialized] public int score = 0;
    int highscore = 0;
    int coinCount = 0;  // Variable to keep track of the number of coins

    private void Awake() {
        instance = this;
    }

    void Start () {
        if (scoreText == null || highscoreText == null || coinText == null) return;

        scoreText.text = score.ToString() + " POINTS";
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        coinText.text = "COINS: " + coinCount.ToString();  // Initialize the coin text
    }

    public void AddPoints (int newPoints) {
        if (scoreText == null || highscoreText == null) return;

        score += newPoints;
        scoreText.text = score.ToString() + " POINTS";

        if (score > highscore) {
            highscore = score;
            highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        }
    }

    public void AddCoin(int newCoins) {
        if (coinText == null) return;

        coinCount += newCoins;
        coinText.text = "COINS: " + coinCount.ToString();  // Update the coin text
    }
}

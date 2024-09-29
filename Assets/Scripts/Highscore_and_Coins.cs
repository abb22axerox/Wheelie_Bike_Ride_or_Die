using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager instance;
    public bool loadDataOnStart;
    public Text scoreText;
    public Text highscoreText;
    public Text coinText;  // New Text component for displaying the coin count

    [NonSerialized] public int score = 0;
    int highscore = 0;
    int coins = 0;
    float timer = 0;

    private void Awake() {
        instance = this;
    }

    void Start () {
        if (loadDataOnStart)
        {
            highscore = PlayerPrefs.GetInt("HighScore");
            coins = PlayerPrefs.GetInt("Coins");
        }

        if (scoreText == null || highscoreText == null || coinText == null) return;

        scoreText.text = score.ToString() + " POINTS";
        highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        coinText.text = "COINS: " + coins.ToString();  // Initialize the coin text
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
        {
            timer -= 1.0f;

            AddPoints(1);
        }
    }

    public void AddPoints (int newPoints) {
        if (scoreText == null || highscoreText == null) return;

        score += newPoints;
        scoreText.text = score.ToString() + " POINTS";

        if (score > highscore) {
            highscore = score;
            PlayerPrefs.SetInt("OwnedStatus", highscore);
            PlayerPrefs.Save();
            highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        }
    }

    public void AddCoin(int newCoins) {
        if (coinText == null) return;

        AddPoints(5);
        coins++;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();

        coinText.text = "COINS: " + coins.ToString();  // Update the coin text
    }
}

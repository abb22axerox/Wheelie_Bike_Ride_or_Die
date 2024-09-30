using System;
using UnityEngine;
using TMPro;
 
public class ScoreManager : MonoBehaviour {
 
    public ScoreManager instance;
    public PlayerController player;
    float PointDstThreshold = 5;
    public bool loadDataOnStart;
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text coinText;  // New Text component for displaying the coin count
 
    [NonSerialized] public int score = 0;
    int highscore = 0;
    public int coins = 0;
    float timer = 0;
    float last = 0;
 
    private void Awake() {
        instance = this;
    }
 
    void Start () {
        if (loadDataOnStart)
        {
            highscore = PlayerPrefs.GetInt("HighScore");
            coins = PlayerPrefs.GetInt("Coins");
        }
 
        if (scoreText != null) scoreText.text = score.ToString();
        if (coinText != null) coinText.text = coins.ToString();
        if (highscoreText != null) highscoreText.text = "HIGHSCORE: " + highscore.ToString();
    }
 
    void Update()
    {
        timer += Time.deltaTime * player.SPEED;
        if (timer > PointDstThreshold && Time.deltaTime != 0)
        {
            timer = 0;
 
            AddPoints(1);
        }
    }
 
    public void AddPoints (int newPoints) {
        if (scoreText == null) return;
 
        score += newPoints;
        scoreText.text = score.ToString();
 
        if (score > highscore) {
            highscore = score;
            PlayerPrefs.SetInt("HighScore", highscore);
            PlayerPrefs.Save();
            if (highscoreText == null) return;
            highscoreText.text = "HIGHSCORE: " + highscore.ToString();
        }
    }
 
    public void AddCoin(int newCoins) {
        if (coinText == null) return;
 
        AddPoints(50);
        coins += newCoins;
        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();
        Debug.Log(coins);
 
        coinText.text = coins.ToString();
    }
}
 
 
 
 
 
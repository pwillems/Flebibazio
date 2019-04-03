using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text scoreText;
    public Text highScoreText;
    private int score;
    private int highscore;

	// Use this for initialization
	void Start () {
        score = 0;
        UpdateScore();
        highscore = PlayerPrefs.GetInt("highscore");
        UpdateHighScore();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateHighScore()
    {
        highScoreText.text = highscore.ToString();
    }

    void UpdateScore()
    {
        scoreText.text = score.ToString();

        // Check if we beat high score:
        if(score > highscore)
        {
            highscore = score;
            UpdateHighScore();
            PlayerPrefs.SetInt("highscore", highscore);
        }

    }

    public void AddScore(int additionalScoreValue)
    {
        score += additionalScoreValue;
        UpdateScore();
    }

    private void LoadHighScores()
    {
        
    }
}

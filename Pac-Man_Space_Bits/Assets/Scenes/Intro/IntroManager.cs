using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [SerializeField] GameData gameData;

    [SerializeField] Text HighScoreText;

    private void Start()
    {
        int highscore = SaveSystem.LoadHighScore();
        HighScoreText.text = "HIGH SCORE .......... " + highscore;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] PelletsSet pelletsSet;
    [SerializeField] PelletsSet SuperPelletsSet;

    [SerializeField] Text _currentScoreText;
    [SerializeField] Text _maxScoreText;
    [SerializeField] GameObject _gameOverText;

    [SerializeField] GameData gameData;
    [SerializeField] PacMan PacMan;
    [SerializeField] GhostsSet ghostSet;
    // Start is called before the first frame update

    void Start()
    {
        Pellet.onPelletCollected += RaiseScore;
        SuperPellet.onSuperPelletCollected += SuperPelletTime;

        PacMan.onPlayerDeath += ProcessDeath;

        gameData.StartGameData();
        _maxScoreText.text = gameData.MaxScore.ToString();
    }

    void SuperPelletTime() 
    {
        RaiseScore();



    }

    void ProcessDeath()
    {
        PacMan.GetComponent<Movement>().CanMove = false;
        foreach (var item in ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }
    }

    void RaiseScore() 
    {
        gameData.CurrentScore += 100;
        _currentScoreText.text = gameData.CurrentScore.ToString();

        if (gameData.LevelPellets.Items.Count == 0 && gameData.LevelSuperPellets.Items.Count == 0)
        {
            EndGame();
        }
    }

    void EndGame() 
    {
        _maxScoreText.text = gameData.MaxScore.ToString();
        _gameOverText.SetActive(true);
        gameData.EndGameData();
        Time.timeScale = 0f;
    }
}

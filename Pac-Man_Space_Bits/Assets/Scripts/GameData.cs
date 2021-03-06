using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] int _currentScore;
    [SerializeField] int _maxScore;
    [SerializeField] int _currentLives;
    [SerializeField] int _maxLives;
    [SerializeField] PelletsSet _levelPellets;
    [SerializeField] PelletsSet _levelSuperPellets;

    public int CurrentScore { get => _currentScore; set => _currentScore = value; }
    public int MaxScore { get => _maxScore; set => _maxScore = value; }
    public PelletsSet LevelPellets { get => _levelPellets; set => _levelPellets = value; }
    public PelletsSet LevelSuperPellets { get => _levelSuperPellets; set => _levelSuperPellets = value; }
    public int CurrentLives { get => _currentLives; set => _currentLives = value; }
    public int MaxLives { get => _maxLives; set => _maxLives = value; }

    public void StartGameData()
    {
        CurrentScore = 0;
        CurrentLives = MaxLives - 1;
    }

    public void EndGameData()
    {
        if (CurrentScore > MaxScore)
        {
            MaxScore = CurrentScore;
        }

        CurrentScore = 0;
        CurrentLives = MaxLives - 1;
    }
}

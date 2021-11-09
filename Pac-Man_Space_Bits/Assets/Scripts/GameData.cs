using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    [SerializeField] int _currentScore;
    [SerializeField] int _maxScore;
    [SerializeField] PelletsSet _levelPellets;
    [SerializeField] PelletsSet _levelSuperPellets;

    public int CurrentScore { get => _currentScore; set => _currentScore = value; }
    public int MaxScore { get => _maxScore; set => _maxScore = value; }
    public PelletsSet LevelPellets { get => _levelPellets; set => _levelPellets = value; }
    public PelletsSet LevelSuperPellets { get => _levelSuperPellets; set => _levelSuperPellets = value; }

    public void EndGameData() 
    {
        if (_currentScore > _maxScore)
        {
            _maxScore = _currentScore;
        }

        _currentScore = 0;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] PelletsSet _pelletsSet;
    [SerializeField] PelletsSet _superPelletsSet;
    [SerializeField] SceneController _sceneController;

    [SerializeField] Text _currentScoreText;
    [SerializeField] Text _maxScoreText;
    [SerializeField] GameObject _gameOverText;
    [SerializeField] Text _livesHeader;
    [SerializeField] GameObject[] _LivesImages;

    [SerializeField] GameData _gameData;
    [SerializeField] PacMan _pacMan;
    [SerializeField] GhostsSet _ghostSet;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSource _SFXSource;
    [SerializeField] AudioClip _deathClip;
    [SerializeField] AudioClip _superPillClip;
    [SerializeField] AudioClip _gameBeginClip;
    [SerializeField] AudioClip _extraLifeClip;

    bool _isCourotineActive_SuperPellet;
    bool _isGameBeggining;
    UnityEngine.Coroutine PelletTime;

    float GameTimer;
    int savedHighScore;
    bool _alreadyGainedExtraLife;
    int _ghostsEatenInSuccession;

    // Start is called before the first frame update

    void Start()
    {
        Pellet.onPelletCollected += RaiseScore;
        SuperPellet.onSuperPelletCollected += SuperPelletTime;
        SuperPellet.onSuperPelletDone += RaiseScore;

        Ghost.onGhostEaten += GhostScoreSequence;
        PacMan.onPlayerDeath += ProcessDeath;

        _livesHeader.text = "LIVES 1UP";

        savedHighScore = SaveSystem.LoadHighScore();
        _gameData.StartGameData();
        _maxScoreText.text = SaveSystem.LoadHighScore().ToString();
        _currentScoreText.text = _gameData.CurrentScore.ToString();

        for (int i = 0; i < _LivesImages.Length; i++)
        {
            if (_gameData.CurrentLives < i + 1)
            {
                _LivesImages[i].SetActive(false);
            }
        }

        StartCoroutine(BeginGame());
        StartCoroutine(ProcessGameTimer());
    }

    private void OnDisable()
    {
        Pellet.onPelletCollected -= RaiseScore;
        SuperPellet.onSuperPelletCollected -= SuperPelletTime;
        SuperPellet.onSuperPelletDone -= RaiseScore;

        Ghost.onGhostEaten -= GhostScoreSequence;
        PacMan.onPlayerDeath -= ProcessDeath;
    }

    IEnumerator ProcessGameTimer()
    {
        yield return new WaitUntil(() => GameTimer > 7);
        foreach (var item in _ghostSet.Items)
        {
            if (item.GetComponent<Pinky>())
                item.CanLeaveHome = true;

            item.IsChaseMode = true;
        }
        yield return new WaitUntil(() => GameTimer > 12);
        foreach (var item in _ghostSet.Items)
        {
            if (!item.CanLeaveHome)
                item.CanLeaveHome = true;
        }
        yield return new WaitUntil(() => GameTimer > 20);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = false;
        }
        yield return new WaitUntil(() => GameTimer > 27);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = true;
        }
        yield return new WaitUntil(() => GameTimer > 47);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = false;
        }
        yield return new WaitUntil(() => GameTimer > 54);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = true;
        }
        yield return new WaitUntil(() => GameTimer > 61);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = false;
        }
        yield return new WaitUntil(() => GameTimer > 68);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = true;
        }
    }

    private void Update()
    {
        if (!_isGameBeggining)
        {
            if (!_isCourotineActive_SuperPellet)
            {
                GameTimer += Time.deltaTime;
            }
        }
    }

    void SuperPelletTime()
    {
        if (!_isCourotineActive_SuperPellet)
        {
            PelletTime = StartCoroutine(SuperPelletCoroutine());
        }
        else
        {
            StopCoroutine(PelletTime);
            _audioSource.Stop();
            _audioSource.loop = false;
            _pacMan.GetComponent<PacMan>().isPlayerInvincible = false;
            onSuperPelletStop?.Invoke();
            _isCourotineActive_SuperPellet = false;
            _ghostsEatenInSuccession = 0;
            PelletTime = StartCoroutine(SuperPelletCoroutine());
        }

    }

    public static event Action onSuperPelletStop;
    IEnumerator SuperPelletCoroutine()
    {
        _isCourotineActive_SuperPellet = true;
        _audioSource.Stop();
        _audioSource.clip = _superPillClip;
        _audioSource.loop = false;
        _pacMan.GetComponent<PacMan>().isPlayerInvincible = true;
        for (int i = 0; i < 3; i++)
        {
            _audioSource.Play();
            yield return new WaitUntil(() => _audioSource.isPlaying == false);
        }
        _audioSource.Stop();
        _audioSource.loop = false;
        _pacMan.GetComponent<PacMan>().isPlayerInvincible = false;
        onSuperPelletStop?.Invoke();
        _isCourotineActive_SuperPellet = false;
        _ghostsEatenInSuccession = 0;
        yield return null;
    }

    IEnumerator BeginGame()
    {
        _isGameBeggining = true;
        _pacMan.GetComponent<Movement>().CanMove = false;

        foreach (var item in _ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        _audioSource.clip = _gameBeginClip;
        _audioSource.Play();

        yield return new WaitUntil(() => _audioSource.isPlaying == false);

        foreach (var item in _ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = true;
        }

        _pacMan.GetComponent<Movement>().CanMove = true;
        _isGameBeggining = false;
    }

    void ProcessDeath()
    {
        StopCoroutine(PelletTime);
        _audioSource.Stop();
        _audioSource.loop = false;
        _pacMan.GetComponent<PacMan>().isPlayerInvincible = false;
        onSuperPelletStop?.Invoke();
        _isCourotineActive_SuperPellet = false;
        _ghostsEatenInSuccession = 0;

        _pacMan.GetComponent<PacMan>().DiePacMan();

        foreach (var item in _ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        _audioSource.Stop();
        _audioSource.clip = _deathClip;
        StartCoroutine(ProccessDeathcorotine());
    }

    IEnumerator ProccessDeathcorotine()
    {
        _audioSource.loop = false;
        _audioSource.Play();
        yield return new WaitUntil(() => _audioSource.isPlaying == false);

        if (_gameData.CurrentLives > 0)
        {
            _gameData.CurrentLives--;
            for (int i = 0; i < _LivesImages.Length; i++)
            {
                if (_gameData.CurrentLives < i + 1)
                {
                    _LivesImages[i].SetActive(false);
                }
            }
            ResetLevel();
        }
        else
        {
            SaveHighScore();
            EndGame();
        }
    }

    void ResetLevel()
    {
        _pacMan.transform.position = _pacMan.GetComponent<PacMan>().startPosition;
        _pacMan.GetComponent<PacMan>().ResetPacMan();
        foreach (var item in _ghostSet.Items)
        {
            item.GetComponent<Ghost>().ResetGhost();

        }
    }

    void GhostScoreSequence(int value)
    {
        if (_ghostsEatenInSuccession <= 4)
            _ghostsEatenInSuccession++;

        if (_ghostsEatenInSuccession == 0)
            RaiseScore(200);
        else
            RaiseScore(200 ^ _ghostsEatenInSuccession);
    }

    void RaiseScore(int score)
    {
        _gameData.CurrentScore += score;

        if (!_alreadyGainedExtraLife)
        {
            if (_gameData.CurrentScore > 10000)
            {
                _livesHeader.text = "LIVES 0UP";
                _gameData.CurrentLives++;
                _alreadyGainedExtraLife = true;
                _SFXSource.clip = _extraLifeClip;
                _SFXSource.Play();
                for (int i = 0; i < _LivesImages.Length; i++)
                {
                    if (_gameData.CurrentLives > i)
                    {
                        _LivesImages[i].SetActive(true);
                    }
                }
            }
        }


        _currentScoreText.text = _gameData.CurrentScore.ToString();

        if (_gameData.LevelPellets.Items.Count == 0 && _gameData.LevelSuperPellets.Items.Count == 0)
        {
            SaveHighScore();
            EndGame();
        }
    }

    void SaveHighScore()
    {
        if (_gameData.CurrentScore > savedHighScore)
        {
            SaveSystem.SaveHighScore(_gameData.CurrentScore);
            savedHighScore = _gameData.CurrentScore;
        }
    }

    void EndGame()
    {
        foreach (var item in _ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        _pacMan.GetComponent<Movement>().CanMove = false;
        _pacMan.audioSource.Stop();

        _maxScoreText.text = savedHighScore.ToString();
        _gameOverText.SetActive(true);
        _gameData.EndGameData();
        _pacMan.GetComponent<AudioSource>().Stop();
        StartCoroutine(GoBackToMenu());
    }

    IEnumerator GoBackToMenu()
    {
        yield return new WaitForSecondsRealtime(2f);
        _sceneController.LoadSceneByName("Intro");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Text _currentScoreText;
    [SerializeField] Text _maxScoreText;
    [SerializeField] GameObject _gameOverText;
    [SerializeField] Text _livesHeader;
    [SerializeField] GameObject[] _LivesImages;
    [SerializeField] GameObject PauseScreen;
    [SerializeField] EventSystem _eventSystem;

    [Header("Data")]
    [SerializeField] GameData _gameData;
    [SerializeField] PacMan _pacMan;
    [SerializeField] GhostsSet _ghostSet;
    [SerializeField] PelletsSet _pelletsSet;
    [SerializeField] PelletsSet _superPelletsSet;
    [SerializeField] SceneController _sceneController;

    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioSource _behaviourAudioSource;
    [SerializeField] AudioSource _SFXSource;
    [SerializeField] AudioClip _deathClip;
    [SerializeField] AudioClip _superPillClip;
    [SerializeField] AudioClip _gameBeginClip;
    [SerializeField] AudioClip _extraLifeClip;
    [SerializeField] AudioClip _scatterClip;
    [SerializeField] AudioClip _chaseClip;

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
        Time.timeScale = 1f;

        Pellet.onPelletCollected += RaiseScore;
        SuperPellet.onSuperPelletCollected += SuperPelletTime;
        SuperPellet.onSuperPelletDone += RaiseScore;

        Ghost.onGhostEaten += GhostScoreSequence;
        PacMan.onPlayerDeath += ProcessDeath;

        PlayerInputHandler.onPauseInputEvent += PauseGame;

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

    }

    private void OnDisable()
    {
        Pellet.onPelletCollected -= RaiseScore;
        SuperPellet.onSuperPelletCollected -= SuperPelletTime;
        SuperPellet.onSuperPelletDone -= RaiseScore;

        Ghost.onGhostEaten -= GhostScoreSequence;
        PacMan.onPlayerDeath -= ProcessDeath;
        PlayerInputHandler.onPauseInputEvent -= PauseGame;
    }

    void PauseGame()
    {
        if (_isGameBeggining) return;

        _audioSource.Pause();
        _behaviourAudioSource.Pause();
        PauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        _behaviourAudioSource.Play();
        _audioSource.Play();
        PauseScreen.SetActive(false);
    }

    IEnumerator ProcessGameTimer()
    {
        _behaviourAudioSource.Stop();
        _behaviourAudioSource.clip = _scatterClip;
        _behaviourAudioSource.Play();
        yield return new WaitUntil(() => GameTimer > 7);
        foreach (var item in _ghostSet.Items)
        {
            if (item.GetComponent<Pinky>())
                item.CanLeaveHome = true;

            item.IsChaseMode = true;
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _chaseClip;
            _behaviourAudioSource.Play();
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
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _scatterClip;
            _behaviourAudioSource.Play();
        }
        yield return new WaitUntil(() => GameTimer > 27);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = true;
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _chaseClip;
            _behaviourAudioSource.Play();
        }
        yield return new WaitUntil(() => GameTimer > 47);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = false;
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _scatterClip;
            _behaviourAudioSource.Play();
        }
        yield return new WaitUntil(() => GameTimer > 54);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = true;
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _chaseClip;
            _behaviourAudioSource.Play();
        }
        yield return new WaitUntil(() => GameTimer > 61);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = false;
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _scatterClip;
            _behaviourAudioSource.Play();
        }
        yield return new WaitUntil(() => GameTimer > 68);
        foreach (var item in _ghostSet.Items)
        {
            item.IsChaseMode = true;
            _behaviourAudioSource.Stop();
            _behaviourAudioSource.clip = _chaseClip;
            _behaviourAudioSource.Play();
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
            if (PelletTime != null)
            {
                StopCoroutine(PelletTime);
                _audioSource.Stop();
                _audioSource.loop = false;
                _pacMan.GetComponent<PacMan>().isPlayerInvincible = false;
                onSuperPelletStop?.Invoke();
                _isCourotineActive_SuperPellet = false;
                _ghostsEatenInSuccession = 0;
                _behaviourAudioSource.Play();
                PelletTime = StartCoroutine(SuperPelletCoroutine());
            }
        }

    }

    public static event Action onSuperPelletStop;
    IEnumerator SuperPelletCoroutine()
    {
        _behaviourAudioSource.Stop();
        _isCourotineActive_SuperPellet = true;
        _audioSource.Stop();
        _audioSource.clip = _superPillClip;
        _audioSource.loop = true;
        _pacMan.GetComponent<PacMan>().isPlayerInvincible = true;
        _audioSource.Play();

        yield return new WaitForSeconds(10f);
        _audioSource.Stop();
        _audioSource.loop = false;
        _pacMan.GetComponent<PacMan>().isPlayerInvincible = false;
        onSuperPelletStop?.Invoke();
        _isCourotineActive_SuperPellet = false;
        _ghostsEatenInSuccession = 0;
        _behaviourAudioSource.Play();
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
        _audioSource.clip = null;
        _isGameBeggining = false;
        StartCoroutine(ProcessGameTimer());
    }

    void ProcessDeath()
    {
        if (PelletTime != null)
        {
            StopCoroutine(PelletTime);
            _audioSource.Stop();
            _audioSource.loop = false;
            _pacMan.GetComponent<PacMan>().isPlayerInvincible = false;
            onSuperPelletStop?.Invoke();
            _isCourotineActive_SuperPellet = false;
            _ghostsEatenInSuccession = 0;
            _behaviourAudioSource.Play();
        }

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

        if (_ghostsEatenInSuccession <= 0)
            RaiseScore(200);
        else if (_ghostsEatenInSuccession == 1)
            RaiseScore(200);
        else if (_ghostsEatenInSuccession == 2)
            RaiseScore(400);
        else if (_ghostsEatenInSuccession == 3)
            RaiseScore(800);
        else if (_ghostsEatenInSuccession == 4)
            RaiseScore(1600);
    }

    void RaiseScore(int score)
    {
        _gameData.CurrentScore += score;

        if (!_alreadyGainedExtraLife)
        {
            if (_gameData.CurrentScore > 1000)
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

    [ContextMenu("EndGame")]
    void EndGame()
    {
        PlayerInputHandler.onPauseInputEvent -= PauseGame;

        foreach (var item in _ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        _behaviourAudioSource.Stop();

        if (PelletTime != null)
        {
            StopCoroutine(PelletTime);
        }

        _audioSource.Stop();
        _audioSource.loop = false;

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

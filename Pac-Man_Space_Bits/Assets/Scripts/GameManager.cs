using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] PelletsSet pelletsSet;
    [SerializeField] PelletsSet SuperPelletsSet;
    [SerializeField] SceneController sceneController;

    [SerializeField] Text _currentScoreText;
    [SerializeField] Text _maxScoreText;
    [SerializeField] GameObject _gameOverText;
    [SerializeField] Text _livesHeader;
    [SerializeField] GameObject[] _LivesImages;

    [SerializeField] GameData gameData;
    [SerializeField] PacMan PacMan;
    [SerializeField] GhostsSet ghostSet;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip SuperPillClip;
    [SerializeField] AudioClip GameBeginClip;

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
        gameData.StartGameData();
        _maxScoreText.text = SaveSystem.LoadHighScore().ToString();
        _currentScoreText.text = gameData.CurrentScore.ToString();

        for (int i = 0; i < _LivesImages.Length; i++)
        {
            if (gameData.CurrentLives < i + 1)
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
        foreach (var item in ghostSet.Items)
        {
            if (item.GetComponent<Pinky>())
                item.CanLeaveHome = true;

            item.IsChaseMode = true;
        }
        yield return new WaitUntil(() => GameTimer > 12);
        foreach (var item in ghostSet.Items)
        {
            if (!item.CanLeaveHome)
                item.CanLeaveHome = true;
        }
        yield return new WaitUntil(() => GameTimer > 20);
        foreach (var item in ghostSet.Items)
        {
            item.IsChaseMode = false;
        }
        yield return new WaitUntil(() => GameTimer > 27);
        foreach (var item in ghostSet.Items)
        {
            item.IsChaseMode = true;
        }
        yield return new WaitUntil(() => GameTimer > 47);
        foreach (var item in ghostSet.Items)
        {
            item.IsChaseMode = false;
        }
        yield return new WaitUntil(() => GameTimer > 54);
        foreach (var item in ghostSet.Items)
        {
            item.IsChaseMode = true;
        }
        yield return new WaitUntil(() => GameTimer > 61);
        foreach (var item in ghostSet.Items)
        {
            item.IsChaseMode = false;
        }
        yield return new WaitUntil(() => GameTimer > 68);
        foreach (var item in ghostSet.Items)
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
            PelletTime = StartCoroutine(SuperPelletCoroutine());
        }

    }

    public static event Action onSuperPelletStop;
    IEnumerator SuperPelletCoroutine()
    {
        _isCourotineActive_SuperPellet = true;
        audioSource.Stop();
        audioSource.clip = SuperPillClip;
        audioSource.loop = false;
        PacMan.GetComponent<PacMan>().isPlayerInvincible = true;
        for (int i = 0; i < 3; i++)
        {
            audioSource.Play();
            yield return new WaitUntil(() => audioSource.isPlaying == false);
        }
        audioSource.Stop();
        audioSource.loop = false;
        PacMan.GetComponent<PacMan>().isPlayerInvincible = false;
        onSuperPelletStop?.Invoke();
        _isCourotineActive_SuperPellet = false;
        _ghostsEatenInSuccession = 0;
        yield return null;
    }

    IEnumerator BeginGame()
    {
        _isGameBeggining = true;
        PacMan.GetComponent<Movement>().CanMove = false;

        foreach (var item in ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        audioSource.clip = GameBeginClip;
        audioSource.Play();

        yield return new WaitUntil(() => audioSource.isPlaying == false);

        foreach (var item in ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = true;
        }

        PacMan.GetComponent<Movement>().CanMove = true;
        _isGameBeggining = false;
    }

    void ProcessDeath()
    {
        PacMan.GetComponent<PacMan>().DiePacMan();

        foreach (var item in ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        audioSource.clip = deathClip;
        StartCoroutine(ProccessDeathcorotine());
    }

    IEnumerator ProccessDeathcorotine()
    {
        audioSource.loop = false;
        audioSource.Play();
        yield return new WaitUntil(() => audioSource.isPlaying == false);

        if (gameData.CurrentLives > 0)
        {
            gameData.CurrentLives--;
            for (int i = 0; i < _LivesImages.Length; i++)
            {
                if (gameData.CurrentLives < i + 1)
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
        PacMan.transform.position = PacMan.GetComponent<PacMan>().startPosition;
        PacMan.GetComponent<PacMan>().ResetPacMan();
        foreach (var item in ghostSet.Items)
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
        gameData.CurrentScore += score;

        if (!_alreadyGainedExtraLife)
        {
            if (gameData.CurrentScore > 10000)
            {
                _livesHeader.text = "LIVES 0UP";
                gameData.CurrentLives++;
                _alreadyGainedExtraLife = true;
                for (int i = 0; i < _LivesImages.Length; i++)
                {
                    if (gameData.CurrentLives > i)
                    {
                        _LivesImages[i].SetActive(true);
                    }
                }
            }
        }


        _currentScoreText.text = gameData.CurrentScore.ToString();

        if (gameData.LevelPellets.Items.Count == 0 && gameData.LevelSuperPellets.Items.Count == 0)
        {
            SaveHighScore();
            EndGame();
        }
    }

    void SaveHighScore()
    {
        if (gameData.CurrentScore > savedHighScore)
        {
            SaveSystem.SaveHighScore(gameData.CurrentScore);
            savedHighScore = gameData.CurrentScore;
        }
    }

    void EndGame()
    {
        foreach (var item in ghostSet.Items)
        {
            item.GetComponent<Movement>().CanMove = false;
        }

        PacMan.GetComponent<Movement>().CanMove = false;
        PacMan.audioSource.Stop();

        _maxScoreText.text = savedHighScore.ToString();
        _gameOverText.SetActive(true);
        gameData.EndGameData();
        PacMan.GetComponent<AudioSource>().Stop();
        StartCoroutine(GoBackToMenu());
    }

    IEnumerator GoBackToMenu()
    {
        yield return new WaitForSecondsRealtime(2f);
        sceneController.LoadSceneByName("Intro");
    }
}

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
    [SerializeField] GameObject[] _LivesImages;

    [SerializeField] GameData gameData;
    [SerializeField] PacMan PacMan;
    [SerializeField] GhostsSet ghostSet;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip SuperPillClip;
    [SerializeField] AudioClip GameBeginClip;

    bool isCourotineActive_SuperPellet;
    UnityEngine.Coroutine PelletTime;
    // Start is called before the first frame update

    void Start()
    {
        Pellet.onPelletCollected += RaiseScore;
        SuperPellet.onSuperPelletCollected += SuperPelletTime;
        SuperPellet.onSuperPelletDone += RaiseScore;

        Ghost.onGhostEaten += RaiseScore;
        PacMan.onPlayerDeath += ProcessDeath;

        gameData.StartGameData();
        _maxScoreText.text = gameData.MaxScore.ToString();
        _currentScoreText.text = gameData.CurrentScore.ToString();

        StartCoroutine(BeginGame());
    }

    private void OnDisable()
    {
        Pellet.onPelletCollected -= RaiseScore;
        SuperPellet.onSuperPelletCollected -= SuperPelletTime;
        SuperPellet.onSuperPelletDone -= RaiseScore;

        Ghost.onGhostEaten -= RaiseScore;
        PacMan.onPlayerDeath -= ProcessDeath;
    }

    void SuperPelletTime()
    {
        if (!isCourotineActive_SuperPellet)
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

        isCourotineActive_SuperPellet = true;
        audioSource.Stop();
        audioSource.clip = SuperPillClip;
        audioSource.Play();
        audioSource.loop = true;
        PacMan.GetComponent<PacMan>().isPlayerInvincible = true;
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        audioSource.Stop();
        audioSource.loop = false;
        PacMan.GetComponent<PacMan>().isPlayerInvincible = false;
        onSuperPelletStop?.Invoke();
        isCourotineActive_SuperPellet = false;
        yield return null;

    }

    IEnumerator BeginGame() 
    {
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
        if (gameData.CurrentLives > 0)
        {
            gameData.CurrentScore *= gameData.CurrentLives;
        }

        _maxScoreText.text = gameData.MaxScore.ToString();
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

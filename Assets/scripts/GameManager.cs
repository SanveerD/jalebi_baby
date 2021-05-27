using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void GameDelegate();
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnRestart;

    public static GameManager Instance;

    public GameObject startPage;
    public GameObject gamePage;
    public GameObject gameOverPage;
    public GameObject creditsPage;
    public Text scoreText;
    public AudioSource introSound;
    public AudioSource startSound;

    int score = 0;
    int highscore = 0;
    bool gameOver = true;
    public bool GameOver{ get { return gameOver; } }
    public int GetScore { get { return score; } }

    enum PageState
    {
        GamePage,
        StartPage,
        GameOverPage,
        CreditsPage
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        SetPageState(PageState.StartPage);
        introSound.Play();
    }
    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();

        if (startPage.active || creditsPage.active)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
        }

        if (gameOverPage.active)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartGame();
            }
        }
    }

    void OnEnable()
    {
        PlayerMovement.OnPlayerDied += OnPlayerDied;
        PlayerMovement.OnPlayerScored += OnPlayerScored;
    }

    void OnDisable()
    {
        PlayerMovement.OnPlayerDied -= OnPlayerDied;
        PlayerMovement.OnPlayerScored += OnPlayerScored;
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.StartPage:
                startPage.SetActive(true);
                gamePage.SetActive(false);
                gameOverPage.SetActive(false);
                creditsPage.SetActive(false);
                break;
            case PageState.GamePage:
                startPage.SetActive(false);
                gamePage.SetActive(true);
                gameOverPage.SetActive(false);
                creditsPage.SetActive(false);
                break;
            case PageState.GameOverPage:
                startPage.SetActive(false);
                gamePage.SetActive(true);
                gameOverPage.SetActive(true);
                creditsPage.SetActive(false);
                break;
            case PageState.CreditsPage:
                startPage.SetActive(false);
                gamePage.SetActive(false);
                gameOverPage.SetActive(false);
                creditsPage.SetActive(true);
                break;
        }
    }

    public void StartGame()
    {
        score = 0;
        gameOver = false;
        SetPageState(PageState.GamePage);
        OnGameStarted();
        startSound.Play();
    }

    public void RestartGame()
    {
        SetPageState(PageState.StartPage);
        introSound.Play();
        OnRestart();
    }

    public void Credits()
    {
        SetPageState(PageState.CreditsPage);
    }

    void OnPlayerScored()
    {
        score++;

    }

    void OnPlayerDied()
    {
        if (score > highscore)
        {
            PlayerPrefs.SetInt("highscore", score);
            highscore = score;
        }
        gameOver = true;
        SetPageState(PageState.GameOverPage);
    }
}

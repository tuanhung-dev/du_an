using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameMode
{
    Classic,
    TimeAttack,
    Challenge
}

public enum GameState
{
    Home,
    Mode,
    Playing,
    Paused,
    GameOver,
    YouWin
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private const string AutoStartKey = "AUTO_START_GAME";
    private const string LastModeKey = "LAST_GAME_MODE";

    [Header("Panels")]
    public GameObject homePanel;
    public GameObject modePanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject youWinPanel;

    [Header("Gameplay References")]
    public Spawner spawner;
    public Board board;
    public GameObject gridBoard;

    [Header("HUD Texts")]
    public TMP_Text modeText;
    public TMP_Text timerText;
    public TMP_Text challengeText;

    [Header("Time Attack Settings")]
    public float timeAttackDuration = 120f;

    [Header("Challenge Settings")]
    public float challengeDuration = 60f;
    public int challengeTargetLines = 10;

    [Header("Current")]
    public GameMode currentMode = GameMode.Classic;
    public GameState currentState = GameState.Home;

    private bool hasStartedGame = false;
    private float currentTime;
    private float challengeTime;

    private void Awake()
    {
        Instance = this;

        if (spawner == null)
        {
            spawner = FindFirstObjectByType<Spawner>();
        }

        if (board == null)
        {
            board = FindFirstObjectByType<Board>();
        }
    }

    private void Start()
    {
        currentMode = (GameMode)PlayerPrefs.GetInt(LastModeKey, (int)GameMode.Classic);

        bool autoStart = PlayerPrefs.GetInt(AutoStartKey, 0) == 1;

        if (autoStart)
        {
            PlayerPrefs.SetInt(AutoStartKey, 0);
            PlayerPrefs.Save();

            StartGame();
        }
        else
        {
            ShowHome();
        }
    }

    private void Update()
    {
        if (currentState != GameState.Playing) return;

        UpdateTimeAttackMode();
        UpdateChallengeMode();
    }

    private void HideAllPanels()
    {
        if (homePanel != null) homePanel.SetActive(false);
        if (modePanel != null) modePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (youWinPanel != null) youWinPanel.SetActive(false);
    }

    private void SetGameplayVisible(bool value)
    {
        if (gridBoard != null)
        {
            gridBoard.SetActive(value);
        }
    }

    private void ShowPanelOnTop(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
    }

    private void CacheReferences()
    {
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<Spawner>();
        }

        if (board == null)
        {
            board = FindFirstObjectByType<Board>();
        }
    }

    public void ShowHome()
    {
        currentState = GameState.Home;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(false);

        ShowPanelOnTop(homePanel);
    }

    public void ShowModePanel()
    {
        currentState = GameState.Mode;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(false);

        ShowPanelOnTop(modePanel);
    }

    public void BackToHome()
    {
        ShowHome();
    }

    public void PlayClassic()
    {
        currentMode = GameMode.Classic;
        StartGame();
    }

    public void PlayTimeAttack()
    {
        currentMode = GameMode.TimeAttack;
        StartGame();
    }

    public void PlayChallenge()
    {
        currentMode = GameMode.Challenge;
        StartGame();
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;

        HideAllPanels();
        SetGameplayVisible(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
            hudPanel.transform.SetAsFirstSibling();
        }

        CacheReferences();

        PlayerPrefs.SetInt(LastModeKey, (int)currentMode);
        PlayerPrefs.Save();

        SetupModeUI();

        if (!hasStartedGame)
        {
            hasStartedGame = true;

            if (spawner != null)
            {
                spawner.StartNewGame();
            }
            else
            {
                Debug.LogError("Chưa kéo Spawner vào GameManager!");
            }
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBGM();
        }

        Debug.Log("Start Game - Mode: " + currentMode);
    }

    private void SetupModeUI()
    {
        currentTime = timeAttackDuration;
        challengeTime = challengeDuration;

        if (modeText != null)
        {
            if (currentMode == GameMode.Classic)
            {
                modeText.text = "Classic";
            }
            else if (currentMode == GameMode.TimeAttack)
            {
                modeText.text = "Time Attack";
            }
            else if (currentMode == GameMode.Challenge)
            {
                modeText.text = "Challenge";
            }
        }

        // TimerText chỉ hiện ở chế độ tính giờ
        if (timerText != null)
        {
            bool isTimeAttack = currentMode == GameMode.TimeAttack;
            timerText.gameObject.SetActive(isTimeAttack);

            if (isTimeAttack)
            {
                timerText.text = FormatTime(currentTime);
            }
        }

        if (challengeText != null)
        {
            bool isChallenge = currentMode == GameMode.Challenge;
            challengeText.gameObject.SetActive(isChallenge);

            if (isChallenge)
            {
                challengeText.text = "Thử thách: 0/" + challengeTargetLines;
            }
        }
    }

    private void UpdateTimeAttackMode()
    {
        if (currentMode != GameMode.TimeAttack) return;

        currentTime -= Time.deltaTime;

        if (currentTime < 0)
        {
            currentTime = 0;
        }

        if (timerText != null)
        {
            timerText.text = FormatTime(currentTime);
        }

        if (currentTime <= 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateChallengeMode()
    {
        if (currentMode != GameMode.Challenge) return;
        if (board == null) return;

        challengeTime -= Time.deltaTime;

        if (challengeTime < 0)
        {
            challengeTime = 0;
        }

        int currentLines = board.TotalLines;

        if (challengeText != null)
        {
            challengeText.text = "Thử thách: " + currentLines + "/" + challengeTargetLines;
        }

        // Hoàn thành thử thách bằng cách xóa đủ dòng
        if (currentLines >= challengeTargetLines)
        {
            ShowYouWin();
            return;
        }

        // Hoặc sống sót hết thời gian thử thách
        if (challengeTime <= 0)
        {
            ShowYouWin();
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Paused;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
            hudPanel.transform.SetAsFirstSibling();
        }

        ShowPanelOnTop(pausePanel);
    }

    public void ContinueGame()
    {
        if (currentState != GameState.Paused) return;

        currentState = GameState.Playing;
        Time.timeScale = 1f;

        HideAllPanels();
        SetGameplayVisible(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
    }

    public void ShowGameOver()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
            hudPanel.transform.SetAsFirstSibling();
        }

        ShowPanelOnTop(gameOverPanel);
    }

    public void ShowYouWin()
    {
        currentState = GameState.YouWin;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
            hudPanel.transform.SetAsFirstSibling();
        }

        ShowPanelOnTop(youWinPanel);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetInt(AutoStartKey, 1);
        PlayerPrefs.SetInt(LastModeKey, (int)currentMode);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        Time.timeScale = 1f;

        PlayerPrefs.SetInt(AutoStartKey, 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    // Hàm dự phòng nếu nút cũ vẫn đang gọi PlayLevel
    public void PlayLevel()
    {
        PlayChallenge();
    }

    public void BackToHomePanel()
    {
        BackToHome();
    }

    public void Restart()
    {
        RestartGame();
    }

    public void Home()
    {
        GoHome();
    }
}
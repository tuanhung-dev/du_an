using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Classic,
    TimeAttack,
    Level
}

public enum GameState
{
    Home,
    Mode,
    Playing,
    Paused,
    GameOver
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

    [Header("Gameplay References")]
    public Spawner spawner;
    public GameObject gridBoard;

    [Header("Current")]
    public GameMode currentMode = GameMode.Classic;
    public GameState currentState = GameState.Home;

    private bool hasStartedGame = false;

    private void Awake()
    {
        Instance = this;

        if (spawner == null)
        {
            spawner = FindFirstObjectByType<Spawner>();
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

    private void HideAllPanels()
    {
        if (homePanel != null) homePanel.SetActive(false);
        if (modePanel != null) modePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void SetGameplayVisible(bool isVisible)
    {
        if (gridBoard != null)
        {
            gridBoard.SetActive(isVisible);
        }
    }

    private void CacheSpawner()
    {
        if (spawner != null) return;

        if (gridBoard != null)
        {
            spawner = gridBoard.GetComponent<Spawner>();
        }

        if (spawner == null)
        {
            spawner = FindFirstObjectByType<Spawner>();
        }
    }

    private void ShowPanelOnTop(GameObject panel)
    {
        if (panel == null) return;

        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
    }

    public void ShowHome()
    {
        currentState = GameState.Home;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(false);

        ShowPanelOnTop(homePanel);

        Debug.Log("Show Home");
    }

    public void ShowModePanel()
    {
        currentState = GameState.Mode;
        Time.timeScale = 0f;

        HideAllPanels();
        SetGameplayVisible(false);

        ShowPanelOnTop(modePanel);

        Debug.Log("Show Mode Panel");
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

    public void PlayLevel()
    {
        currentMode = GameMode.Level;
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

        PlayerPrefs.SetInt(LastModeKey, (int)currentMode);
        PlayerPrefs.Save();

        CacheSpawner();

        if (!hasStartedGame)
        {
            hasStartedGame = true;

            if (spawner != null)
            {
                Debug.Log("GameManager gọi Spawner.StartNewGame()");
                spawner.StartNewGame();
            }
            else
            {
                Debug.LogError("LỖI: Chưa kéo Spawner vào GameManager!");
            }
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBGM();
        }

        Debug.Log("Start Game - Mode: " + currentMode);
    }

    public void PauseGame()
    {
        if (currentState != GameState.Playing)
        {
            Debug.LogWarning("Không pause được vì currentState = " + currentState);
            return;
        }

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

        Debug.Log("Pause Game");
    }

    public void ContinueGame()
    {
        if (currentState != GameState.Paused)
        {
            Debug.LogWarning("Không continue được vì currentState = " + currentState);
            return;
        }

        currentState = GameState.Playing;
        Time.timeScale = 1f;

        HideAllPanels();
        SetGameplayVisible(true);

        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }

        Debug.Log("Continue Game");
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

        Debug.Log("Show Game Over");
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game");

        Time.timeScale = 1f;

        PlayerPrefs.SetInt(AutoStartKey, 1);
        PlayerPrefs.SetInt(LastModeKey, (int)currentMode);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoHome()
    {
        Debug.Log("Go Home");

        Time.timeScale = 1f;

        PlayerPrefs.SetInt(AutoStartKey, 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Hàm dự phòng nếu nút cũ đang gắn tên khác
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

    public void Play()
    {
        PlayClassic();
    }
}
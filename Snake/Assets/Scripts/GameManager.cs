
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // =========================
    // SNAKE SPEED
    // =========================

    public static float snakeSpeed = 0.2f;

    // =========================
    // SNAKE
    // =========================

    [Header("Snake")]
    public Snake snake;

    // =========================
    // PANELS
    // =========================

    [Header("Panels")]
    public GameObject mainMenuPanel;

    public GameObject difficultyPanel;

    public GameObject gameOverPanel;

    // =========================
    // UI
    // =========================

    [Header("UI")]
    public TMP_Text scoreText;

    public TMP_Text highScoreText;

    // =========================
    // OBSTACLE
    // =========================

    [Header("Obstacle")]
    public GameObject obstaclePrefab;

    private List<GameObject> obstacles =
        new List<GameObject>();

    // =========================
    // GAME DATA
    // =========================

    private int score = 0;

    private int highScore = 0;

    private bool isGameOver = false;

    private int currentObstacleAmount = 0;

    // =========================
    // AWAKE
    // =========================

    private void Awake()
    {
        instance = this;
    }

    // =========================
    // START
    // =========================

    private void Start()
    {
        mainMenuPanel.SetActive(true);

        difficultyPanel.SetActive(false);

        gameOverPanel.SetActive(false);

        // Ẩn UI điểm khi ở menu
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(false);
        }

        // High Score luôn bắt đầu từ 0
        highScore = 0;

        score = 0;

        UpdateScore();

        Time.timeScale = 1f;
    }

    // =========================
    // START GAME
    // =========================

    public void StartGame()
    {
        mainMenuPanel.SetActive(false);

        difficultyPanel.SetActive(false);

        gameOverPanel.SetActive(false);

        // Hiện UI điểm
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
        }

        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(true);
        }

        score = 0;

        UpdateScore();

        // Reset rắn
        snake.ResetState();

        // Spawn obstacle
        SpawnObstacles(currentObstacleAmount);

        isGameOver = false;

        Time.timeScale = 1f;
    }

    // =========================
    // OPEN DIFFICULTY
    // =========================

    public void OpenDifficulty()
    {
        difficultyPanel.SetActive(true);
    }

    // =========================
    // SPAWN OBSTACLES
    // =========================

    void SpawnObstacles(int amount)
    {
        // Xóa obstacle cũ
        foreach (GameObject obstacle
            in obstacles)
        {
            Destroy(obstacle);
        }

        obstacles.Clear();

        if (obstaclePrefab == null)
        {
            return;
        }

        for (int i = 0; i < amount; i++)
        {
            bool validPosition = false;

            Vector3 spawnPosition = Vector3.zero;

            while (!validPosition)
            {
                float x =
                    Mathf.Round(Random.Range(-7f, 7f));

                float y =
                    Mathf.Round(Random.Range(-4f, 4f));

                spawnPosition =
                    new Vector3(x, y, 0f);

                // Không spawn gần đầu rắn
                if (Vector3.Distance(
                    spawnPosition,
                    snake.transform.position) > 2.5f)
                {
                    validPosition = true;

                    // Không spawn đè obstacle khác
                    foreach (GameObject obstacle
                        in obstacles)
                    {
                        if (Vector3.Distance(
                            spawnPosition,
                            obstacle.transform.position) < 1f)
                        {
                            validPosition = false;

                            break;
                        }
                    }
                }
            }

            GameObject newObstacle =
                Instantiate(
                    obstaclePrefab,
                    spawnPosition,
                    Quaternion.identity);

            obstacles.Add(newObstacle);
        }
    }

    // =========================
    // DIFFICULTY
    // =========================

    public void Easy()
    {
        snakeSpeed = 0.22f;

        currentObstacleAmount = 0;

        difficultyPanel.SetActive(false);
    }

    public void Medium()
    {
        snakeSpeed = 0.18f;

        currentObstacleAmount = 0;

        difficultyPanel.SetActive(false);
    }

    public void Hard()
    {
        snakeSpeed = 0.14f;

        currentObstacleAmount = 4;

        difficultyPanel.SetActive(false);
    }

    public void VeryHard()
    {
        snakeSpeed = 0.11f;

        currentObstacleAmount = 7;

        difficultyPanel.SetActive(false);
    }

    // =========================
    // SCORE
    // =========================

    public void AddScore(int point)
    {
        score += point;

        // Cập nhật High Score
        if (score > highScore)
        {
            highScore = score;
        }

        UpdateScore();
    }

    // =========================
    // UPDATE SCORE UI
    // =========================

    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + score;
        }

        if (highScoreText != null)
        {
            highScoreText.text =
                "High Score: " + highScore;
        }
    }

    // =========================
    // GAME OVER
    // =========================

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================
    // RESTART GAME
    // =========================

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);

        score = 0;

        UpdateScore();

        snake.ResetState();

        SpawnObstacles(currentObstacleAmount);

        isGameOver = false;

        Time.timeScale = 1f;
    }

    // =========================
    // BACK TO MENU
    // =========================

    public void BackToMenu()
    {
        mainMenuPanel.SetActive(true);

        difficultyPanel.SetActive(false);

        gameOverPanel.SetActive(false);

        // Ẩn UI điểm
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(false);
        }

        // Xóa obstacle
        foreach (GameObject obstacle
            in obstacles)
        {
            Destroy(obstacle);
        }

        obstacles.Clear();

        // Reset điểm
        score = 0;

        highScore = 0;

        UpdateScore();

        Time.timeScale = 1f;
    }

    // =========================
    // QUIT GAME
    // =========================

    public void QuitGame()
    {
        Application.Quit();
    }
}


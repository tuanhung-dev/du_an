using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Tốc độ rắn
    public static float snakeSpeed = 0.2f;

    [Header("Snake")]
    public Snake snake;

    [Header("Panels")]
    public GameObject mainMenuPanel;

    public GameObject difficultyPanel;

    public GameObject gameOverPanel;

    public GameObject winPanel;

    [Header("UI")]
    public TMP_Text scoreText;

    [Header("Obstacle")]
    public GameObject obstaclePrefab;

    private List<GameObject> obstacles = new List<GameObject>();

    private int score = 0;

    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainMenuPanel.SetActive(true);

        difficultyPanel.SetActive(false);

        gameOverPanel.SetActive(false);

        winPanel.SetActive(false);

        // Ẩn điểm ở menu
        scoreText.gameObject.SetActive(false);

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

        winPanel.SetActive(false);

        // Hiện điểm khi chơi
        scoreText.gameObject.SetActive(true);

        score = 0;

        UpdateScore();

        snake.ResetState();

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
    // SPAWN OBSTACLE
    // =========================

    void SpawnObstacles(int amount)
    {
        // Xóa obstacle cũ
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        obstacles.Clear();

        // Spawn obstacle mới
        for (int i = 0; i < amount; i++)
        {
            float x = Mathf.Round(Random.Range(-8f, 8f));

            float y = Mathf.Round(Random.Range(-4f, 4f));

            Vector3 position = new Vector3(x, y, 0f);

            GameObject newObstacle = Instantiate(obstaclePrefab, position, Quaternion.identity);

            obstacles.Add(newObstacle);
        }
    }

    // =========================
    // DIFFICULTY
    // =========================

    public void Easy()
    {
        // Chậm - dễ chơi
        snakeSpeed = 0.22f;

        SpawnObstacles(0);

        difficultyPanel.SetActive(false);
    }

    public void Medium()
    {
        // Bình thường
        snakeSpeed = 0.18f;

        SpawnObstacles(0);

        difficultyPanel.SetActive(false);
    }

    public void Hard()
    {
        // Khó hơn
        snakeSpeed = 0.14f;

        SpawnObstacles(5);

        difficultyPanel.SetActive(false);
    }

    public void VeryHard()
    {
        // Rất khó
        snakeSpeed = 0.11f;

        SpawnObstacles(8);

        difficultyPanel.SetActive(false);
    }

    // =========================
    // SCORE
    // =========================

    public void AddScore(int point)
    {
        score += point;

        UpdateScore();

        // Điều kiện thắng
        if (score >= 100)
        {
            WinGame();
        }
    }

    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
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
    // WIN GAME
    // =========================

    public void WinGame()
    {
        winPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================
    // RESTART
    // =========================

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);

        winPanel.SetActive(false);

        // Hiện lại điểm
        scoreText.gameObject.SetActive(true);

        score = 0;

        UpdateScore();

        snake.ResetState();

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

        winPanel.SetActive(false);

        // Ẩn điểm ở menu
        scoreText.gameObject.SetActive(false);

        // Xóa obstacle khi về menu
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }

        obstacles.Clear();

        score = 0;

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
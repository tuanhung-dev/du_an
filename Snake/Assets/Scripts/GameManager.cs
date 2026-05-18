using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gameOverPanel;

    [Header("Game Objects")]
    public GameObject snake;
    public GameObject foodSpawner;

    [Header("Score")]
    public TextMeshProUGUI scoreText;

    // Điểm
    private int score = 0;

    // KIỂM TRA RESTART
    public static bool isRestarting = false;

    void Start()
    {
        // Ẩn game over
        gameOverPanel.SetActive(false);

        // NẾU ĐANG RESTART
        if (isRestarting)
        {
            StartGame();

            // reset lại
            isRestarting = false;

            return;
        }

        // HIỆN MENU LẦN ĐẦU
        mainMenuPanel.SetActive(true);

        // Tắt gameplay
        snake.SetActive(false);
        foodSpawner.SetActive(false);

        // Dừng game
        Time.timeScale = 0f;

        // Reset score
        score = 0;
        UpdateScore();
    }

    // START GAME
    public void StartGame()
    {
        // Ẩn menu
        mainMenuPanel.SetActive(false);

        // Hiện gameplay
        snake.SetActive(true);
        foodSpawner.SetActive(true);

        // Reset điểm
        score = 0;
        UpdateScore();

        // Chạy game
        Time.timeScale = 1f;
    }

    // THÊM ĐIỂM
    public void AddScore(int amount)
    {
        score += amount;

        UpdateScore();
    }

    // CẬP NHẬT UI SCORE
    void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // GAME OVER
    public void GameOver()
    {
        gameOverPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    // RESTART GAME
    public void RestartGame()
    {
        // Đánh dấu đang restart
        isRestarting = true;

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
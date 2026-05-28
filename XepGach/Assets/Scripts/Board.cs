using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    [Header("Board Size")]
    public int width = 10;
    public int height = 20;

    [Header("Grid")]
    public Transform[,] grid;

    [Header("Score UI")]
    public TMP_Text scoreText;
    public TMP_Text maxScoreText;
    public TMP_Text levelText;

    [Header("Level Settings")]
    public int totalLinesCleared = 0;
    public int level = 1;
    public float currentFallSpeed = 1f;

    private int score = 0;
    private int highScore = 0;
    private bool isGameOver = false;

    public int Score => score;
    public int HighScore => highScore;
    public int TotalLines => totalLinesCleared;
    public int Level => level;

    private void Awake()
    {
        grid = new Transform[width, height];
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    private void Start()
    {
        UpdateAllUI();
    }

    // =====================================================
    // KIỂM TRA VỊ TRÍ KHỐI GẠCH
    // =====================================================

    public bool IsValidPosition(Transform piece)
    {
        foreach (Transform child in piece)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);

            if (pos.x < 0 || pos.x >= width)
            {
                return false;
            }

            if (pos.y < 0)
            {
                return false;
            }

            if (pos.y < height)
            {
                if (grid[pos.x, pos.y] != null &&
                    grid[pos.x, pos.y].parent != piece)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // =====================================================
    // LƯU KHỐI GẠCH VÀO GRID
    // =====================================================

    public void AddToGrid(Transform piece)
    {
        if (isGameOver) return;

        foreach (Transform child in piece)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);

            if (pos.y >= height)
            {
                GameOver();
                return;
            }

            if (pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height)
            {
                grid[pos.x, pos.y] = child;
            }
        }
    }

    // =====================================================
    // KIỂM TRA HÀNG ĐẦY
    // =====================================================

    public void CheckForLines()
    {
        if (isGameOver) return;

        int clearedLines = 0;

        for (int y = 0; y < height; y++)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                MoveLinesDown(y);

                y--;
                clearedLines++;
            }
        }

        if (clearedLines > 0)
        {
            AddScore(clearedLines);
            AddLines(clearedLines);

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.clearLineSound);
            }
        }
    }

    // =====================================================
    // KIỂM TRA HÀNG FULL
    // =====================================================

    private bool IsLineFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    // =====================================================
    // XÓA HÀNG
    // =====================================================

    private void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    // =====================================================
    // DỜI HÀNG XUỐNG
    // =====================================================

    private void MoveLinesDown(int startY)
    {
        for (int y = startY + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;

                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }

    // =====================================================
    // TÍNH ĐIỂM
    // Mỗi dòng = 10 điểm
    // =====================================================

    private void AddScore(int clearedLines)
    {
        int addScore = clearedLines * 10;

        score += addScore;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();

        Debug.Log("Score: " + score);
    }

    // =====================================================
    // TĂNG SỐ DÒNG + LEVEL
    // =====================================================

    private void AddLines(int lines)
    {
        totalLinesCleared += lines;

        int newLevel = (totalLinesCleared / 10) + 1;

        if (newLevel > level)
        {
            level = newLevel;

            currentFallSpeed = Mathf.Max(0.1f, 1f - ((level - 1) * 0.1f));

            Debug.Log("Level Up: " + level + " | Fall Speed: " + currentFallSpeed);
        }

        UpdateLevelUI();
    }

    // =====================================================
    // UPDATE UI
    // =====================================================

    public void UpdateAllUI()
    {
        UpdateScoreUI();
        UpdateLevelUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (maxScoreText != null)
        {
            maxScoreText.text = highScore.ToString();
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = level.ToString();
        }
    }

    // =====================================================
    // PAUSE BUTTON
    // =====================================================

    public void OnPauseButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
    }

    // =====================================================
    // GAME OVER
    // =====================================================

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopBGM();
            AudioManager.instance.PlaySFX(AudioManager.instance.gameOverSound);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOver();
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}
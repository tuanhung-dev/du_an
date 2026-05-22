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
    public TMP_Text linesText;
    public TMP_Text levelText;

    [Header("Level Settings")]
    public int totalLinesCleared = 0;
    public int level = 1;
    public float currentFallSpeed = 1f;

    private int score = 0;
    private int highScore = 0;

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

            // Ra ngoài trái / phải
            if (pos.x < 0 || pos.x >= width)
            {
                return false;
            }

            // Rơi xuống dưới đáy
            if (pos.y < 0)
            {
                return false;
            }

            // Nếu nằm trong board thì kiểm tra có bị trùng gạch không
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
        foreach (Transform child in piece)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);

            // Nếu gạch bị khóa ở phía trên màn hình thì game over
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
    // KIỂM TRA 1 HÀNG CÓ FULL KHÔNG
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
    // DỜI CÁC HÀNG PHÍA TRÊN XUỐNG
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
    // =====================================================

    private void AddScore(int clearedLines)
    {
        int addScore = 0;

        switch (clearedLines)
        {
            case 1:
                addScore = 100;
                break;

            case 2:
                addScore = 300;
                break;

            case 3:
                addScore = 500;
                break;

            default:
                addScore = 800;
                break;
        }

        score += addScore;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
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

            // Level càng cao, gạch rơi càng nhanh
            currentFallSpeed = Mathf.Max(0.1f, 1f - ((level - 1) * 0.1f));

            Debug.Log("Level Up: " + level + " | Fall Speed: " + currentFallSpeed);
        }

        UpdateLinesUI();
        UpdateLevelUI();
    }

    // =====================================================
    // UPDATE UI
    // =====================================================

    private void UpdateAllUI()
    {
        UpdateScoreUI();
        UpdateLinesUI();
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

    private void UpdateLinesUI()
    {
        if (linesText != null)
        {
            linesText.text = totalLinesCleared.ToString();
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
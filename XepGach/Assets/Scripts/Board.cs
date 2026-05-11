using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    public int width = 10;
    public int height = 20;

    public Transform[,] grid;

    [Header("Game")]
    public GameObject gameOverPanel;

    [Header("Score")]
    public TMP_Text scoreText;

    private int score = 0;

    [Header("Speed")]
    public float currentFallSpeed = 1f;

    void Awake()
    {
        grid = new Transform[width, height];
    }

    // =====================================================
    // KIỂM TRA VỊ TRÍ
    // =====================================================

    public bool IsValidPosition(Transform piece)
    {
        foreach (Transform child in piece)
        {
            Vector2Int pos =
                Vector2Int.RoundToInt(child.position);

            // Ra ngoài trái phải
            if (pos.x < 0 || pos.x >= width)
            {
                return false;
            }

            // Xuống dưới đáy
            if (pos.y < 0)
            {
                return false;
            }

            // Ô đã có gạch
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

        // =================================================
        // TĂNG ĐIỂM
        // =================================================

        if (clearedLines > 0)
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

                case 4:
                    addScore = 800;
                    break;
            }

            score += addScore;

            UpdateScoreUI();

            Debug.Log("Score: " + score);
        }
    }

    // =====================================================
    // KIỂM TRA HÀNG FULL
    // =====================================================

    bool IsLineFull(int y)
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

    void ClearLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            // FIX NULL ERROR
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

    void MoveLinesDown(int startY)
    {
        for (int y = startY + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    // Dời dữ liệu trong grid
                    grid[x, y - 1] = grid[x, y];

                    grid[x, y] = null;

                    // Dời object xuống
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }

    // =====================================================
    // UPDATE SCORE UI
    // =====================================================

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // =====================================================
    // GAME OVER
    // =====================================================

    public void GameOver()
    {
        Debug.Log("GAME OVER");

        Time.timeScale = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}
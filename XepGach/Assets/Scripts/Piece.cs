using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board;
    public TetrominoData tetrominoData;

    public float stepDelay = 1f;
    private float moveTimer;

    public int rotationIndex = 0;

    void Start()
    {
        // Tự tìm Board nếu chưa gán
        if (board == null)
        {
            board = FindFirstObjectByType<Board>();
        }
    }

    void Update()
    {
        // Nếu thiếu Board thì tránh lỗi
        if (board == null) return;

        // Rơi tự động
        if (Time.time >= moveTimer)
        {
            Move(Vector2Int.down);
            moveTimer = Time.time + stepDelay;
        }

        // Điều khiển bàn phím
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            Spawner spawner = FindFirstObjectByType<Spawner>();

            if (spawner != null)
            {
                spawner.Hold();
            }
        }
    }

    // =====================================================
    // MOBILE BUTTONS
    // =====================================================

    public void MoveLeft()
    {
        Move(Vector2Int.left);
    }

    public void MoveRight()
    {
        Move(Vector2Int.right);
    }

    public void SoftDrop()
    {
        if (Move(Vector2Int.down))
        {
            moveTimer = Time.time + stepDelay;
        }
    }

    // =====================================================
    // MOVE
    // =====================================================

    bool Move(Vector2Int translation)
    {
        if (!this.enabled || board == null) return false;

        Vector3 newPos = transform.position;

        newPos.x += translation.x;
        newPos.y += translation.y;

        transform.position = newPos;

        if (!board.IsValidPosition(this.transform))
        {
            transform.position -= new Vector3(translation.x, translation.y, 0);

            if (translation == Vector2Int.down)
            {
                Lock();
            }

            return false;
        }

        return true;
    }

    // =====================================================
    // ROTATE
    // =====================================================

    public void Rotate()
    {
        if (!this.enabled) return;

        if (board == null) return;

        // Lưu trạng thái cũ
        int originalRotation = rotationIndex;
        Vector3 originalPos = transform.position;

        // Xoay thử
        rotationIndex = (rotationIndex + 1) % 4;

        transform.eulerAngles -= new Vector3(0, 0, 90);

        bool wallKickSuccess = TestWallKicks(rotationIndex);

        // Âm thanh xoay
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.moveSound);
        }

        // Nếu xoay thất bại thì trả lại
        if (!wallKickSuccess)
        {
            rotationIndex = originalRotation;

            transform.position = originalPos;

            transform.eulerAngles += new Vector3(0, 0, 90);
        }
    }

    // =====================================================
    // WALL KICK
    // =====================================================

    private bool TestWallKicks(int targetRotationIndex)
    {
        if (board == null) return false;

        Vector2Int[,] wallKicksData;

        // Gạch O không cần wall kick
        if (tetrominoData.type == TetrominoType.O)
        {
            return true;
        }
        // Gạch I dùng bảng riêng
        else if (tetrominoData.type == TetrominoType.I)
        {
            wallKicksData = Data.WallKicksI;
        }
        // Các gạch còn lại
        else
        {
            wallKicksData = Data.WallKicksJLOSTZ;
        }

        // Thử 5 vị trí wall kick
        for (int i = 0; i < 5; i++)
        {
            Vector2Int translation =
                wallKicksData[targetRotationIndex, i];

            transform.position +=
                new Vector3(translation.x, translation.y, 0);

            if (board.IsValidPosition(this.transform))
            {
                return true;
            }

            transform.position -=
                new Vector3(translation.x, translation.y, 0);
        }

        return false;
    }

    // =====================================================
    // HARD DROP
    // =====================================================

    public void HardDrop()
    {
        if (!this.enabled || board == null) return;

        while (board.IsValidPosition(this.transform))
        {
            transform.position += Vector3.down;
        }

        transform.position += Vector3.up;

        // Âm thanh hard drop
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(
                AudioManager.instance.hardDropSound
            );
        }

        Lock();
    }

    // =====================================================
    // LOCK
    // =====================================================

    void Lock()
    {
        if (board == null) return;

        foreach (Transform child in transform)
        {
            Vector2Int pos =
                Vector2Int.RoundToInt(child.position);

            board.grid[pos.x, pos.y] = child;
        }

        this.enabled = false;

        board.CheckForLines();

        Spawner spawner =
            FindFirstObjectByType<Spawner>();

        if (spawner != null)
        {
            spawner.SpawnPiece();
        }
    }
}
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board;
    public TetrominoData tetrominoData;

    [Header("Fall Settings")]
    public float stepDelay = 1f;

    private float moveTimer;
    public int rotationIndex = 0;

    private void Start()
    {
        moveTimer = Time.time + stepDelay;
    }

    private void Update()
    {
        if (!CanControlPiece())
        {
            return;
        }

        // Rơi tự động
        if (Time.time >= moveTimer)
        {
            Move(Vector2Int.down);
            moveTimer = Time.time + stepDelay;
        }

        // Điều khiển bằng bàn phím
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            SoftDrop();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
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

    private bool CanControlPiece()
    {
        if (!enabled) return false;

        if (GameManager.Instance != null &&
            GameManager.Instance.currentState != GameState.Playing)
        {
            return false;
        }

        return true;
    }

    // =====================================================
    // HÀM PUBLIC CHO NÚT MOBILE
    // =====================================================

    public void MoveLeft()
    {
        if (!CanControlPiece()) return;
        Move(Vector2Int.left);
    }

    public void MoveRight()
    {
        if (!CanControlPiece()) return;
        Move(Vector2Int.right);
    }

    public void SoftDrop()
    {
        if (!CanControlPiece()) return;

        if (Move(Vector2Int.down))
        {
            moveTimer = Time.time + stepDelay;
        }
    }

    public void HardDrop()
    {
        if (!CanControlPiece()) return;

        while (board.IsValidPosition(transform))
        {
            transform.position += Vector3.down;
        }

        transform.position += Vector3.up;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.hardDropSound);
        }

        Lock();
    }

    // =====================================================
    // DI CHUYỂN
    // =====================================================

    private bool Move(Vector2Int translation)
    {
        if (!enabled) return false;

        Vector3 oldPosition = transform.position;

        transform.position += new Vector3(translation.x, translation.y, 0f);

        if (!board.IsValidPosition(transform))
        {
            transform.position = oldPosition;

            if (translation == Vector2Int.down)
            {
                Lock();
            }

            return false;
        }

        if (translation != Vector2Int.down)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.moveSound);
            }
        }

        return true;
    }

    // =====================================================
    // XOAY KHỐI
    // =====================================================

    public void Rotate()
    {
        if (!CanControlPiece()) return;

        int oldRotationIndex = rotationIndex;
        Vector3 oldPosition = transform.position;

        rotationIndex = (rotationIndex + 1) % 4;
        transform.eulerAngles -= new Vector3(0f, 0f, 90f);

        bool canRotate = TestWallKicks(rotationIndex);

        if (!canRotate)
        {
            rotationIndex = oldRotationIndex;
            transform.position = oldPosition;
            transform.eulerAngles += new Vector3(0f, 0f, 90f);
        }
        else
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.moveSound);
            }
        }
    }

    private bool TestWallKicks(int targetRotationIndex)
    {
        if (tetrominoData.type == TetrominoType.O)
        {
            return true;
        }

        Vector2Int[,] wallKicksData;

        if (tetrominoData.type == TetrominoType.I)
        {
            wallKicksData = Data.WallKicksI;
        }
        else
        {
            wallKicksData = Data.WallKicksJLOSTZ;
        }

        for (int i = 0; i < 5; i++)
        {
            Vector2Int translation = wallKicksData[targetRotationIndex, i];

            transform.position += new Vector3(translation.x, translation.y, 0f);

            if (board.IsValidPosition(transform))
            {
                return true;
            }

            transform.position -= new Vector3(translation.x, translation.y, 0f);
        }

        return false;
    }

    // =====================================================
    // KHÓA KHỐI GẠCH
    // =====================================================

    private void Lock()
    {
        if (!enabled) return;

        board.AddToGrid(transform);

        enabled = false;

        board.CheckForLines();

        if (GameManager.Instance != null &&
            GameManager.Instance.currentState == GameState.GameOver)
        {
            return;
        }

        Spawner spawner = FindFirstObjectByType<Spawner>();

        if (spawner != null)
        {
            spawner.SpawnPiece();
        }
    }
}
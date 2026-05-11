using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Board board;
    public TetrominoData[] tetrominoes;

    public Vector3 spawnPosition = new Vector3(4f, 18f, 0f);

    public Piece activePiece;
    public Ghost ghost;

    [Header("UI Preview Settings")]
    public float previewScale = 0.5f;

    [Header("Next & Hold Settings")]
    public Transform[] nextPositions;

    private TetrominoData[] nextQueue = new TetrominoData[3];
    private GameObject[] nextPreviews = new GameObject[3];

    public Transform holdPosition;

    private TetrominoData holdData;
    private GameObject holdPreview;

    private bool canHold = true;
    private bool hasHeldPiece = false;

    void Start()
    {
        // Tự tìm Board nếu chưa gán
        if (board == null)
        {
            board = FindFirstObjectByType<Board>();
        }

        // Tạo queue ban đầu
        for (int i = 0; i < 3; i++)
        {
            nextQueue[i] = tetrominoes[Random.Range(0, tetrominoes.Length)];
        }

        UpdateNextQueueUI();

        SpawnPiece();
    }

    // =====================================================
    // SPAWN PIECE
    // =====================================================

    public void SpawnPiece()
    {
        TetrominoData data = nextQueue[0];

        // Đẩy queue
        nextQueue[0] = nextQueue[1];
        nextQueue[1] = nextQueue[2];
        nextQueue[2] = tetrominoes[Random.Range(0, tetrominoes.Length)];

        UpdateNextQueueUI();

        InstantiateAndSetup(data);

        canHold = true;
    }

    // =====================================================
    // TẠO GẠCH
    // =====================================================

    private void InstantiateAndSetup(TetrominoData data)
    {
        GameObject pieceObj =
            Instantiate(data.prefab, spawnPosition, Quaternion.identity);

        Piece piece = pieceObj.GetComponent<Piece>();

        // GÁN DỮ LIỆU QUAN TRỌNG
        piece.board = board;
        piece.tetrominoData = data;

        // Tốc độ rơi
        if (board != null)
        {
            piece.stepDelay = board.currentFallSpeed;
        }

        activePiece = piece;

        // Ghost
        if (ghost != null)
        {
            ghost.TrackPiece(piece);
        }

        // Check Game Over
        if (board != null &&
            !board.IsValidPosition(pieceObj.transform))
        {
            Debug.Log("GAME OVER!");

            Destroy(pieceObj);

            board.GameOver();
        }
    }

    // =====================================================
    // HOLD
    // =====================================================

    public void Hold()
    {
        if (Time.timeScale == 0f) return;

        if (!canHold) return;

        if (activePiece == null) return;

        TetrominoData currentData = activePiece.tetrominoData;

        Destroy(activePiece.gameObject);

        if (!hasHeldPiece)
        {
            holdData = currentData;

            hasHeldPiece = true;

            SpawnPiece();
        }
        else
        {
            TetrominoData temp = holdData;

            holdData = currentData;

            InstantiateAndSetup(temp);
        }

        UpdateHoldUI();

        canHold = false;
    }

    // =====================================================
    // NEXT UI
    // =====================================================

    private void UpdateNextQueueUI()
    {
        for (int i = 0; i < 3; i++)
        {
            // Xóa preview cũ
            if (nextPreviews[i] != null)
            {
                Destroy(nextPreviews[i]);
            }

            // Tạo preview mới
            nextPreviews[i] =
                Instantiate(
                    nextQueue[i].prefab,
                    nextPositions[i].position,
                    Quaternion.identity
                );

            // Tắt script Piece để preview không hoạt động
            Piece previewPiece =
                nextPreviews[i].GetComponent<Piece>();

            if (previewPiece != null)
            {
                previewPiece.enabled = false;
            }

            // Thu nhỏ preview
            nextPreviews[i].transform.localScale =
                new Vector3(previewScale, previewScale, 1f);
        }
    }

    // =====================================================
    // HOLD UI
    // =====================================================

    private void UpdateHoldUI()
    {
        if (holdPreview != null)
        {
            Destroy(holdPreview);
        }

        holdPreview =
            Instantiate(
                holdData.prefab,
                holdPosition.position,
                Quaternion.identity
            );

        Piece previewPiece =
            holdPreview.GetComponent<Piece>();

        if (previewPiece != null)
        {
            previewPiece.enabled = false;
        }

        holdPreview.transform.localScale =
            new Vector3(previewScale, previewScale, 1f);
    }

    // =====================================================
    // MOBILE BUTTONS
    // =====================================================

    public void OnRotateButtonClicked()
    {
        if (Time.timeScale == 0f) return;

        if (activePiece == null) return;

        if (!activePiece.enabled) return;

        activePiece.Rotate();
    }

    public void OnLeftButtonClicked()
    {
        if (activePiece != null)
        {
            activePiece.MoveLeft();
        }
    }

    public void OnRightButtonClicked()
    {
        if (activePiece != null)
        {
            activePiece.MoveRight();
        }
    }

    public void OnDownButtonClicked()
    {
        if (activePiece != null)
        {
            activePiece.SoftDrop();
        }
    }

    public void OnDropButtonClicked()
    {
        if (activePiece != null)
        {
            activePiece.HardDrop();
        }
    }

    public void OnHoldButtonClicked()
    {
        Hold();
    }
}
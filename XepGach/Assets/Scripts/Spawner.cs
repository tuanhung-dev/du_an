using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    public Board board;
    public Ghost ghost;

    [Header("Tetromino Data")]
    public TetrominoData[] tetrominoes;

    [Header("Spawn")]
    public Vector3 spawnPosition = new Vector3(4f, 18f, 0f);
    public Piece activePiece;

    [Header("UI Preview Settings")]
    public float previewScale = 0.5f;

    [Header("Next Settings")]
    public Transform[] nextPositions;

    [Header("Hold Settings")]
    public Transform holdPosition;

    private TetrominoData[] nextQueue = new TetrominoData[3];
    private GameObject[] nextPreviews = new GameObject[3];

    private TetrominoData holdData;
    private GameObject holdPreview;

    private bool canHold = true;
    private bool hasHeldPiece = false;
    private bool hasStarted = false;

    // GameManager sẽ gọi hàm này khi bấm Play
    public void StartNewGame()
    {
        if (hasStarted) return;

        hasStarted = true;

        for (int i = 0; i < nextQueue.Length; i++)
        {
            nextQueue[i] = GetRandomTetromino();
        }

        UpdateNextQueueUI();
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (!hasStarted) return;

        TetrominoData data = nextQueue[0];

        nextQueue[0] = nextQueue[1];
        nextQueue[1] = nextQueue[2];
        nextQueue[2] = GetRandomTetromino();

        UpdateNextQueueUI();

        InstantiateAndSetup(data);

        canHold = true;
    }

    private TetrominoData GetRandomTetromino()
    {
        return tetrominoes[Random.Range(0, tetrominoes.Length)];
    }

    private void InstantiateAndSetup(TetrominoData data)
    {
        GameObject pieceObj = Instantiate(data.prefab, spawnPosition, Quaternion.identity);

        Piece piece = pieceObj.GetComponent<Piece>();

        piece.board = board;
        piece.stepDelay = board.currentFallSpeed;
        piece.tetrominoData = data;

        activePiece = piece;

        if (ghost != null)
        {
            ghost.TrackPiece(piece);
        }

        if (!board.IsValidPosition(pieceObj.transform))
        {
            Destroy(pieceObj);
            board.GameOver();
        }
    }

    public void Hold()
    {
        if (!CanUseHold()) return;

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

    private bool CanUseHold()
    {
        if (!hasStarted) return false;
        if (!canHold) return false;
        if (activePiece == null) return false;
        if (!activePiece.enabled) return false;

        if (GameManager.Instance != null &&
            GameManager.Instance.currentState != GameState.Playing)
        {
            return false;
        }

        return true;
    }

    private void UpdateNextQueueUI()
    {
        for (int i = 0; i < nextPreviews.Length; i++)
        {
            if (nextPreviews[i] != null)
            {
                Destroy(nextPreviews[i]);
            }

            if (nextPositions == null || i >= nextPositions.Length)
            {
                continue;
            }

            if (nextPositions[i] == null || nextQueue[i].prefab == null)
            {
                continue;
            }
            nextPreviews[i] = Instantiate(
                nextQueue[i].prefab,
                nextPositions[i].position,
                Quaternion.identity
            );

            Piece previewPiece = nextPreviews[i].GetComponent<Piece>();

            if (previewPiece != null)
            {
                previewPiece.enabled = false;
            }

            nextPreviews[i].transform.localScale =
                new Vector3(previewScale, previewScale, 1f);
        }
    }

    private void UpdateHoldUI()
    {
        if (holdPreview != null)
        {
            Destroy(holdPreview);
        }

        if (holdPosition == null || !hasHeldPiece || holdData.prefab == null)
        {
            return;
        }

        holdPreview = Instantiate(
            holdData.prefab,
            holdPosition.position,
            Quaternion.identity
        );

        Piece previewPiece = holdPreview.GetComponent<Piece>();

        if (previewPiece != null)
        {
            previewPiece.enabled = false;
        }

        holdPreview.transform.localScale =
            new Vector3(previewScale, previewScale, 1f);
    }

    public void OnRotateButtonClicked()
    {
        if (activePiece == null) return;

        activePiece.Rotate();
    }

    public void OnLeftButtonClicked()
    {
        if (activePiece == null) return;

        activePiece.MoveLeft();
    }

    public void OnRightButtonClicked()
    {
        if (activePiece == null) return;

        activePiece.MoveRight();
    }

    public void OnSoftDropButtonClicked()
    {
        if (activePiece == null) return;

        activePiece.SoftDrop();
    }

    public void OnHardDropButtonClicked()
    {
        if (activePiece == null) return;

        activePiece.HardDrop();
    }

    public void OnHoldButtonClicked()
    {
        Hold();
    }
}
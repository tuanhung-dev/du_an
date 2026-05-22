using UnityEngine;

public class SwipeController : MonoBehaviour
{
    public Spawner spawner;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    private bool isSwiping = false;

    [Header("Swipe Settings")]
    public float minSwipeDistance = 50f;

    private void Update()
    {
        if (!CanUseTouch()) return;

        if (Input.GetMouseButtonDown(0))
        {
            isSwiping = true;
            startTouchPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isSwiping)
        {
            isSwiping = false;
            endTouchPosition = Input.mousePosition;

            AnalyzeTouch();
        }
    }

    private bool CanUseTouch()
    {
        if (spawner == null) return false;
        if (spawner.activePiece == null) return false;
        if (!spawner.activePiece.enabled) return false;

        if (GameManager.Instance != null &&
            GameManager.Instance.currentState != GameState.Playing)
        {
            return false;
        }

        return true;
    }

    private void AnalyzeTouch()
    {
        Vector2 swipeDelta = endTouchPosition - startTouchPosition;

        if (swipeDelta.magnitude > minSwipeDistance)
        {
            // Vuốt dọc
            if (Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
            {
                if (swipeDelta.y > 0)
                {
                    spawner.activePiece.HardDrop();
                }
                else
                {
                    spawner.activePiece.SoftDrop();
                }
            }
            else
            {
                // Vuốt ngang
                if (swipeDelta.x > 0)
                {
                    spawner.activePiece.MoveRight();
                }
                else
                {
                    spawner.activePiece.MoveLeft();
                }
            }
        }
        else
        {
            // Tap nửa trái / nửa phải màn hình
            if (endTouchPosition.x < Screen.width / 2f)
            {
                spawner.activePiece.MoveLeft();
            }
            else
            {
                spawner.activePiece.MoveRight();
            }
        }
    }
}
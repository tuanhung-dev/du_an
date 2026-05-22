using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    // Hướng di chuyển
    private Vector2 direction = Vector2.right;

    // Tốc độ
    public float moveDelay = 0.2f;

    // Prefab thân rắn
    public Transform bodyPrefab;

    // Danh sách thân
    private List<Transform> bodyParts = new List<Transform>();

    // Vị trí ban đầu
    private Vector3 startPosition;

    // Biến kiểm tra ăn food
    private bool shouldGrow = false;

    private void Start()
    {
        startPosition = transform.position;

        ResetState();
    }

    private void Update()
    {
        // Điều khiển
        if (Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
        {
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
        {
            direction = Vector2.right;
        }
    }

    private void Move()
    {
        // Nếu ăn food -> tạo thân ở cuối đuôi
        if (shouldGrow)
        {
            Grow();

            shouldGrow = false;
        }

        // Di chuyển thân
        for (int i = bodyParts.Count - 1; i > 0; i--)
        {
            bodyParts[i].position = bodyParts[i - 1].position;
        }

        // Di chuyển đầu
        transform.position += new Vector3(direction.x, direction.y, 0);
    }

    // Tạo thân mới
    private void Grow()
    {
        Transform body = Instantiate(bodyPrefab);

        body.position = bodyParts[bodyParts.Count - 1].position;

        body.tag = "Body";

        bodyParts.Add(body);
    }

    // Reset game
    public void ResetState()
    {
        CancelInvoke();

        direction = Vector2.right;

        transform.position = startPosition;

        // Xóa thân cũ
        for (int i = 1; i < bodyParts.Count; i++)
        {
            Destroy(bodyParts[i].gameObject);
        }

        bodyParts.Clear();

        bodyParts.Add(transform);

        shouldGrow = false;

        moveDelay = GameManager.snakeSpeed;

        InvokeRepeating(nameof(Move), moveDelay, moveDelay);

        // Spawn food
        FoodSpawner spawner = FindObjectOfType<FoodSpawner>();

        if (spawner != null)
        {
            spawner.SpawnFood();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ăn food
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);

            // Chỉ đánh dấu sẽ tăng thân
            shouldGrow = true;

            // Tăng điểm
            GameManager.instance.AddScore(10);

            // Spawn food mới
            FoodSpawner spawner = FindObjectOfType<FoodSpawner>();

            if (spawner != null)
            {
                spawner.SpawnFood();
            }
        }

        // Chạm tường hoặc thân
        if (other.CompareTag("Wall") || other.CompareTag("Body"))
        {
            GameManager.instance.GameOver();
        }
    }
}
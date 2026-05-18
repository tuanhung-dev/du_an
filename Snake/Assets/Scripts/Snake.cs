using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    // Hướng di chuyển
    private Vector2 direction = Vector2.right;

    // Tốc độ di chuyển
    public float moveDelay = 0.2f;

    // Prefab thân rắn
    public Transform bodyPrefab;

    // Danh sách thân rắn
    private List<Transform> bodyParts = new List<Transform>();

    // Lưu vị trí cũ của đầu rắn
    private Vector3 lastHeadPosition;

    void Start()
    {
        // Cho rắn di chuyển liên tục
        InvokeRepeating(nameof(Move), 0.1f, moveDelay);
    }

    void Update()
    {
        // LÊN
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            && direction != Vector2.down)
        {
            direction = Vector2.up;
        }

        // XUỐNG
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            && direction != Vector2.up)
        {
            direction = Vector2.down;
        }

        // TRÁI
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            && direction != Vector2.right)
        {
            direction = Vector2.left;
        }

        // PHẢI
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            && direction != Vector2.left)
        {
            direction = Vector2.right;
        }
    }

    void Move()
    {
        // Lưu vị trí cũ của đầu rắn
        lastHeadPosition = transform.position;

        // Body cuối đi theo body trước
        for (int i = bodyParts.Count - 1; i > 0; i--)
        {
            bodyParts[i].position = bodyParts[i - 1].position;
        }

        // Body đầu đi theo đầu rắn
        if (bodyParts.Count > 0)
        {
            bodyParts[0].position = transform.position;
        }

        // Đầu rắn di chuyển
        transform.position = new Vector3(
            transform.position.x + direction.x,
            transform.position.y + direction.y,
            0
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ĂN FOOD
        if (other.CompareTag("Food"))
        {
            // Xóa food cũ
            Destroy(other.gameObject);

            // Spawn food mới
            FindFirstObjectByType<FoodSpawner>().SpawnFood();

            // Tăng chiều dài
            Grow();

            // Tăng điểm
            FindFirstObjectByType<GameManager>().AddScore(10);
        }

        // CHẠM THÂN HOẶC TƯỜNG
        if (other.CompareTag("Body") || other.CompareTag("Wall"))
        {
            FindFirstObjectByType<GameManager>().GameOver();
        }
    }

    void Grow()
    {
        // Tạo body mới
        Transform newBody = Instantiate(bodyPrefab);

        // Spawn body mới tại vị trí cũ của đầu rắn
        newBody.position = lastHeadPosition;

        // Thêm vào danh sách
        bodyParts.Add(newBody);
    }
}
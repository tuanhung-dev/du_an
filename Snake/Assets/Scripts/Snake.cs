using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    // Hướng di chuyển
    private Vector2 direction = Vector2.right;

    // Tốc độ di chuyển
    public float moveDelay = 0.2f;

    // Prefab thân rắn
    public Transform bodyPrefab;

    // Panel Game Over
    public GameObject gameOverPanel;

    // Danh sách thân rắn
    private List<Transform> bodyParts = new List<Transform>();

    void Start()
    {
        // Ẩn Game Over lúc bắt đầu
        gameOverPanel.SetActive(false);

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
        // Ăn food
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);

            // Spawn food mới
            FindFirstObjectByType<FoodSpawner>().SpawnFood();

            // Tăng chiều dài
            Grow();
        }

      

        // Chạm tường
        if (other.CompareTag("Wall"))
        {
            GameOver();
        }
    }

    void Grow()
    {
        // Tạo body mới
        Transform newBody = Instantiate(bodyPrefab);

        // Nếu chưa có body
        if (bodyParts.Count == 0)
        {
            newBody.position = transform.position;
        }
        else
        {
            // Spawn tại vị trí body cuối
            newBody.position = bodyParts[bodyParts.Count - 1].position;
        }

        // Thêm vào danh sách
        bodyParts.Add(newBody);
    }

    void GameOver()
    {
        // Hiện panel Game Over
        gameOverPanel.SetActive(true);

        // Dừng game
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        // Chạy lại thời gian
        Time.timeScale = 1;

        // Load lại scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
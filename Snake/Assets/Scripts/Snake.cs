
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    // Hướng di chuyển
    private Vector2 direction = Vector2.right;

    // Tốc độ
    public float moveDelay = 0.2f;

    // Prefab thân
    public Transform bodyPrefab;

    // Danh sách thân
    public List<Transform> bodyParts =
        new List<Transform>();

    // Vị trí ban đầu
    private Vector3 startPosition;

    // Kiểm tra ăn food
    private bool shouldGrow = false;

    private void Start()
    {
        startPosition = transform.position;

        ResetState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)
            && direction != Vector2.down)
        {
            direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.S)
            && direction != Vector2.up)
        {
            direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.A)
            && direction != Vector2.right)
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.D)
            && direction != Vector2.left)
        {
            direction = Vector2.right;
        }
    }

    private void Move()
    {
        if (shouldGrow)
        {
            Grow();

            shouldGrow = false;
        }

        for (int i = bodyParts.Count - 1;
            i > 0;
            i--)
        {
            bodyParts[i].position =
                bodyParts[i - 1].position;
        }

        transform.position +=
            new Vector3(
                direction.x,
                direction.y,
                0);
    }

    private void Grow()
    {
        Transform body =
            Instantiate(bodyPrefab);

        body.position =
            bodyParts[bodyParts.Count - 1].position;

        body.tag = "Body";

        bodyParts.Add(body);
    }

    public void ResetState()
    {
        CancelInvoke();

        direction = Vector2.right;

        transform.position = startPosition;

        for (int i = 1;
            i < bodyParts.Count;
            i++)
        {
            Destroy(bodyParts[i].gameObject);
        }

        bodyParts.Clear();

        bodyParts.Add(transform);

        shouldGrow = false;

        moveDelay = GameManager.snakeSpeed;

        InvokeRepeating(
            nameof(Move),
            moveDelay,
            moveDelay);

        FoodSpawner spawner =
            FindObjectOfType<FoodSpawner>();

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

            shouldGrow = true;

            GameManager.instance.AddScore(10);

            FoodSpawner spawner =
                FindObjectOfType<FoodSpawner>();

            if (spawner != null)
            {
                spawner.SpawnFood();
            }
        }

        // Chạm tường
        if (other.CompareTag("Wall"))
        {
            GameManager.instance.GameOver();
        }

        // Chạm thân
        if (other.CompareTag("Body"))
        {
            GameManager.instance.GameOver();
        }

        // Chạm obstacle
        if (other.CompareTag("Obstacle"))
        {
            GameManager.instance.GameOver();
        }
    }
}


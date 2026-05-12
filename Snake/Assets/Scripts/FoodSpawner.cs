using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;

    public void SpawnFood()
    {
        Vector2 randomPosition;

        do
        {
            randomPosition = new Vector2(
                Random.Range(-5, 5),
                Random.Range(-5, 5)
            );

        } while (Physics2D.OverlapCircle(randomPosition, 0.2f));

        Instantiate(foodPrefab, randomPosition, Quaternion.identity);
    }

    void Start()
    {
        SpawnFood();
    }
}
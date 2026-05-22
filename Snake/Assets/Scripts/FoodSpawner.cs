using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    // Food prefab
    public GameObject foodPrefab;

    // Food hiện tại
    private GameObject currentFood;

    // Spawn food
    public void SpawnFood()
    {
        // Xóa food cũ
        if (currentFood != null)
        {
            Destroy(currentFood);
        }

        // Random vị trí
        float x = Mathf.Round(Random.Range(-8f, 8f));

        float y = Mathf.Round(Random.Range(-4f, 4f));

        Vector3 randomPosition = new Vector3(x, y, 0f);

        // Spawn food mới
        currentFood = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
    }

    private void Start()
    {
        SpawnFood();
    }
}
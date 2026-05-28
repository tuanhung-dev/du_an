
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;

    private GameObject currentFood;

    public void SpawnFood()
    {
        if (currentFood != null)
        {
            Destroy(currentFood);
        }

        float x =
            Mathf.Round(Random.Range(-8f, 8f));

        float y =
            Mathf.Round(Random.Range(-4f, 4f));

        Vector3 spawnPosition =
            new Vector3(x, y, 0f);

        currentFood =
            Instantiate(
                foodPrefab,
                spawnPosition,
                Quaternion.identity);
    }
}


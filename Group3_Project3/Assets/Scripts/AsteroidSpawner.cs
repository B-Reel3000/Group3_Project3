using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidPrefabs;
    public float spawnRate = 1f;
    public float xSpawnRange = 8f;
    public float zSpawnOffset = 30f;

    private float spawnTimer;

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnAsteroid();
            spawnTimer = spawnRate;
        }
    }

    void SpawnAsteroid()
    {
        int randomIndex = Random.Range(0, asteroidPrefabs.Length);

        Vector3 spawnPos = new Vector3(
            Random.Range(-xSpawnRange, xSpawnRange),
            transform.position.y,
            transform.position.z + zSpawnOffset
        );

        Instantiate(asteroidPrefabs[randomIndex], spawnPos, Random.rotation);
    }
}
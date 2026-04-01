using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject laserPowerUpPrefab;
    public float spawnRate = 12f;
    public float xSpawnRange = 8f;
    public float zSpawnOffset = 30f;

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnRate;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnPowerUp();
            spawnTimer = spawnRate;
        }
    }

    void SpawnPowerUp()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(-xSpawnRange, xSpawnRange),
            transform.position.y,
            transform.position.z + zSpawnOffset
        );

        Instantiate(laserPowerUpPrefab, spawnPos, Quaternion.identity);
    }
}
using UnityEngine;

public class ScrapSpawner : MonoBehaviour
{
    public GameObject scrapPrefab;
    public float spawnRate = 6f;
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
            SpawnScrap();
            spawnTimer = spawnRate;
        }
    }

    void SpawnScrap()
    {
        Vector3 spawnPos = new Vector3(
            Random.Range(-xSpawnRange, xSpawnRange),
            transform.position.y,
            transform.position.z + zSpawnOffset
        );

        Instantiate(scrapPrefab, spawnPos, Random.rotation);
    }
}
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public GameObject enemyPrefab;

    [Header("Spawn Timing")]
    public float spawnRate = 4f;
    public int maxEnemiesOnScreen = 4;

    [Header("Lane Setup")]
    public float[] laneXPositions = { -6f, -2f, 2f, 6f };
    public float targetZPosition = 12f;
    public float spawnZPosition = 25f;

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
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemiesOnScreen)
            {
                SpawnEnemyInLane();
            }

            spawnTimer = spawnRate;
        }
    }

    void SpawnEnemyInLane()
    {
        if (enemyPrefab == null || laneXPositions.Length == 0) return;

        List<float> freeLanes = GetFreeLanes();

        if (freeLanes.Count == 0) return;

        float chosenLane = freeLanes[Random.Range(0, freeLanes.Count)];

        Vector3 spawnPos = new Vector3(chosenLane, transform.position.y, spawnZPosition);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.Euler(0f, 180f, 0f));

        Enemy enemyScript = newEnemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            Vector3 targetPos = new Vector3(chosenLane, transform.position.y, targetZPosition);
            enemyScript.SetTargetPosition(targetPos);
        }
    }

    List<float> GetFreeLanes()
    {
        List<float> freeLanes = new List<float>();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (float laneX in laneXPositions)
        {
            bool laneOccupied = false;

            foreach (GameObject enemy in enemies)
            {
                if (Mathf.Abs(enemy.transform.position.x - laneX) < 0.75f)
                {
                    laneOccupied = true;
                    break;
                }
            }

            if (!laneOccupied)
            {
                freeLanes.Add(laneX);
            }
        }

        return freeLanes;
    }
}
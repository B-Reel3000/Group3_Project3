using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float lifeTime = 10f;
    public float rotateSpeed = 100f;

    [Header("Score")]
    public int scoreValue = 10;

    [Header("Effects")]
    public GameObject explosionPrefab;

    private float speedMultiplier = 1f;

    void Start()
    {
        Destroy(gameObject, lifeTime);

        if (GameDataManager.instance != null)
        {
            speedMultiplier = GameDataManager.instance.GetTravelSpeedMultiplier();
        }
    }

    void Update()
    {
        transform.position += Vector3.back * moveSpeed * speedMultiplier * Time.deltaTime;
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage();
            }

            Explode();
            Destroy(gameObject);
        }
    }

    public void DestroyByLaser()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.AddScore(scoreValue);
        }

        Explode();
        Destroy(gameObject);
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
    }
}
using UnityEngine;

public class ScrapPickup : MonoBehaviour
{
    public int scoreValue = 25;
    public float moveSpeed = 8f;
    public float rotateSpeed = 100f;
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameDataManager.instance != null)
            {
                GameDataManager.instance.AddScore(scoreValue);
            }

            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float lifeTime = 10f;
    public float rotateSpeed = 100f;

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
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage();
            }

            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 30f;
    public float lifeTime = 10f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += Vector3.back * speed * Time.deltaTime;
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
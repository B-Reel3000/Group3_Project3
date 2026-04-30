using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 1;

    private Collider bulletCollider;

    void Start()
    {
        bulletCollider = GetComponent<Collider>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && bulletCollider != null)
        {
            Collider[] playerColliders = player.GetComponentsInChildren<Collider>();

            for (int i = 0; i < playerColliders.Length; i++)
            {
                if (playerColliders[i] != null)
                {
                    Physics.IgnoreCollision(bulletCollider, playerColliders[i], true);
                }
            }
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy == null)
            {
                enemy = other.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Boss"))
        {
            BossController boss = other.GetComponent<BossController>();

            if (boss == null)
            {
                boss = other.GetComponentInParent<BossController>();
            }

            if (boss != null)
            {
                boss.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Asteroid"))
        {
            Destroy(gameObject);
        }
    }
}
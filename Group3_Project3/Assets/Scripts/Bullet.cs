using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 1;

    void Start()
    {
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
                Debug.Log("Bullet hit Enemy: " + other.name);
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
                Debug.Log("Bullet hit Boss: " + other.name);
                boss.TakeDamage(damage);
            }
            else
            {
                Debug.Log("Hit object tagged Boss, but no BossController found: " + other.name);
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
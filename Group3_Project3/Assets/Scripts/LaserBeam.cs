using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float damagePerSecond = 2f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            Asteroid asteroid = other.GetComponent<Asteroid>();

            if (asteroid != null)
            {
                asteroid.DestroyByLaser();
            }
            else
            {
                Destroy(other.gameObject);
            }
        }

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy == null)
            {
                enemy = other.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                enemy.TakeLaserDamage(damagePerSecond * Time.deltaTime);
            }
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
                boss.TakeDamage(1);
            }
        }
    }
}
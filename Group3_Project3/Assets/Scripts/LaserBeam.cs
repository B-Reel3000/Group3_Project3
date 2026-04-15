using UnityEngine;

public class LaserBeam : MonoBehaviour
{
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

            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }
    }
}
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Asteroid"))
        {
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
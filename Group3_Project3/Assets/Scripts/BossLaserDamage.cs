using UnityEngine;

public class BossLaserDamage : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage();
            }
        }
    }
}
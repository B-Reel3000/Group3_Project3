using UnityEngine;

public class LaserPowerUp : MonoBehaviour
{
    public float laserAmmoAmount = 50f;
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
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.AddLaserAmmo(laserAmmoAmount);
            }

            Destroy(gameObject);
        }
    }
}
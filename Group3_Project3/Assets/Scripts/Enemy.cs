using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int scoreValue = 50;

    [Header("Shooting")]
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1.5f;

    [Header("Movement")]
    public float enterSpeed = 8f;
    public float strafeSpeed = 2f;
    public float strafeRange = 1.5f;

    [Header("Effects")]
    public GameObject explosionPrefab;

    private float fireTimer;
    private Vector3 targetPosition;
    private Vector3 combatStartPosition;
    private bool hasReachedPosition = false;

    public void SetTargetPosition(Vector3 newTargetPosition)
    {
        targetPosition = newTargetPosition;
        combatStartPosition = newTargetPosition;
    }

    void Update()
    {
        if (!hasReachedPosition)
        {
            MoveIntoPosition();
        }
        else
        {
            Strafe();
            HandleShooting();
        }
    }

    void MoveIntoPosition()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            enterSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;
            hasReachedPosition = true;
        }
    }

    void Strafe()
    {
        float xOffset = Mathf.Sin(Time.time * strafeSpeed) * strafeRange;

        transform.position = new Vector3(
            combatStartPosition.x + xOffset,
            transform.position.y,
            combatStartPosition.z
        );
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            if (enemyBulletPrefab != null && firePoint != null)
            {
                Instantiate(enemyBulletPrefab, firePoint.position, Quaternion.identity);
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.enemyShootSFX);
            }

            fireTimer = fireRate;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        Debug.Log(gameObject.name + " took damage. Health now: " + health);

        if (health <= 0)
        {
            if (GameDataManager.instance != null)
            {
                GameDataManager.instance.AddScore(scoreValue);
            }

            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.explosionSFX);
        }
    }
}
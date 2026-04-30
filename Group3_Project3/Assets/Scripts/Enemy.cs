using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 3;
    public int scoreValue = 50;

    [Header("Laser Damage")]
    public float laserHealth = 3f;
    public Color normalColor = Color.white;
    public Color fullLaserDamageColor = Color.red;

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

    private float currentLaserHealth;
    private Renderer[] renderers;
    private Color[] originalColors;

    void Start()
    {
        currentLaserHealth = laserHealth;

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                originalColors[i] = GetRendererColor(renderers[i]);
            }
        }
    }

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

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeLaserDamage(float damageAmount)
    {
        currentLaserHealth -= damageAmount;

        float damagePercent = 1f - (currentLaserHealth / laserHealth);
        damagePercent = Mathf.Clamp01(damagePercent);

        UpdateLaserDamageColor(damagePercent);

        if (currentLaserHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateLaserDamageColor(float damagePercent)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                Color newColor = Color.Lerp(originalColors[i], fullLaserDamageColor, damagePercent);
                SetRendererColor(renderers[i], newColor);
            }
        }
    }

    void Die()
    {
        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.AddScore(scoreValue);
        }

        Explode();
        Destroy(gameObject);
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

    Color GetRendererColor(Renderer rend)
    {
        if (rend.material.HasProperty("_BaseColor"))
        {
            return rend.material.GetColor("_BaseColor");
        }

        if (rend.material.HasProperty("_Color"))
        {
            return rend.material.color;
        }

        return Color.white;
    }

    void SetRendererColor(Renderer rend, Color color)
    {
        if (rend.material.HasProperty("_BaseColor"))
        {
            rend.material.SetColor("_BaseColor", color);
        }
        else if (rend.material.HasProperty("_Color"))
        {
            rend.material.color = color;
        }
    }
}
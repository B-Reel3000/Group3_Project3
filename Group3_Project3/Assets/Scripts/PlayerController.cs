using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float horizontalLimit = 8f;
    public float verticalLimitMin = -3f;
    public float verticalLimitMax = 3f;

    [Header("Normal Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.15f;
    public float bulletSpawnOffset = 1f;

    [Header("Triple Shot Upgrade")]
    public bool hasTripleShot = false;
    public Transform[] tripleFirePoints;

    [Header("Laser")]
    public GameObject laserBeam;
    public float laserAmmo = 0f;
    public float maxLaserAmmo = 100f;
    public float laserDrainRate = 25f;
    public Slider laserAmmoSlider;

    [Header("Animation")]
    public Animator anim;

    [Header("Health")]
    public int lives = 3;

    [Header("Lives UI")]
    public Image[] lifeIcons;

    [Header("Invincibility")]
    public float invincibleTime = 1f;

    [Header("Level Manager")]
    public LevelManager levelManager;

    [Header("Effects")]
    public GameObject explosionPrefab;

    private Rigidbody rb;
    private float moveInputX;
    private float moveInputZ;
    private float fireTimer;
    private bool isInvincible = false;
    private bool canControl = true;
    private bool laserSoundPlaying = false;
    private int shieldHits = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (GameDataManager.instance != null)
        {
            lives = GameDataManager.instance.GetMaxLives();
            shieldHits = GameDataManager.instance.GetShieldHits();

            // This still uses your old spread upgrade bool, but treats it as triple shot now.
            hasTripleShot = GameDataManager.instance.hasSpreadUpgrade;
        }

        UpdateLivesUI();
        UpdateLaserUI();

        if (laserBeam != null)
        {
            laserBeam.SetActive(false);
        }
    }

    void Update()
    {
        if (!canControl) return;

        moveInputX = Input.GetAxisRaw("Horizontal");
        moveInputZ = Input.GetAxisRaw("Vertical");

        if (anim != null)
        {
            anim.SetFloat("moveInput", moveInputX);
        }

        HandleLaser();
        HandleShooting();
    }

    void FixedUpdate()
    {
        if (!canControl) return;

        Vector3 newPos = rb.position;

        newPos.x += moveInputX * moveSpeed * Time.fixedDeltaTime;
        newPos.z += moveInputZ * moveSpeed * Time.fixedDeltaTime;

        newPos.x = Mathf.Clamp(newPos.x, -horizontalLimit, horizontalLimit);
        newPos.z = Mathf.Clamp(newPos.z, verticalLimitMin, verticalLimitMax);

        rb.MovePosition(newPos);
    }

    void HandleShooting()
    {
        if (IsUsingLaser()) return;

        fireTimer -= Time.deltaTime;

        if (Input.GetButton("Fire1") && fireTimer <= 0f)
        {
            if (hasTripleShot)
            {
                FireTripleShot();
            }
            else
            {
                FireSingleShot();
            }

            fireTimer = fireRate;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.shootSFX);
            }
        }
    }

    void FireSingleShot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector3 spawnPosition = firePoint.position + firePoint.forward * bulletSpawnOffset;
        Instantiate(bulletPrefab, spawnPosition, firePoint.rotation);
    }

    void FireTripleShot()
    {
        if (bulletPrefab == null) return;

        if (tripleFirePoints == null || tripleFirePoints.Length == 0)
        {
            FireSingleShot();
            return;
        }

        for (int i = 0; i < tripleFirePoints.Length; i++)
        {
            if (tripleFirePoints[i] == null) continue;

            Vector3 spawnPosition = tripleFirePoints[i].position + tripleFirePoints[i].forward * bulletSpawnOffset;
            Instantiate(bulletPrefab, spawnPosition, tripleFirePoints[i].rotation);
        }
    }

    void HandleLaser()
    {
        if (laserBeam == null) return;

        if (IsUsingLaser())
        {
            laserBeam.SetActive(true);
            laserAmmo -= laserDrainRate * Time.deltaTime;

            if (!laserSoundPlaying)
            {
                if (AudioManager.instance != null)
                {
                    AudioManager.instance.PlaySFX(AudioManager.instance.laserSFX);
                }

                laserSoundPlaying = true;
            }

            if (laserAmmo < 0f)
            {
                laserAmmo = 0f;
            }

            UpdateLaserUI();
        }
        else
        {
            laserBeam.SetActive(false);
            laserSoundPlaying = false;
        }
    }

    bool IsUsingLaser()
    {
        return Input.GetMouseButton(1) && laserAmmo > 0f;
    }

    public void AddLaserAmmo(float amount)
    {
        laserAmmo += amount;
        laserAmmo = Mathf.Clamp(laserAmmo, 0f, maxLaserAmmo);
        UpdateLaserUI();
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        if (shieldHits > 0)
        {
            shieldHits--;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.hitSFX);
            }

            if (anim != null)
            {
                anim.SetTrigger("TakeHit");
            }

            StartCoroutine(Invincibility());
            return;
        }

        lives--;
        UpdateLivesUI();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.hitSFX);
        }

        if (anim != null)
        {
            anim.SetTrigger("TakeHit");
        }

        StartCoroutine(Invincibility());

        if (lives <= 0)
        {
            Explode();

            if (levelManager != null)
            {
                levelManager.GameOver();
            }

            gameObject.SetActive(false);
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

    IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }

    public void DisableControl()
    {
        canControl = false;

        if (anim != null)
        {
            anim.SetFloat("moveInput", 0f);
        }

        if (laserBeam != null)
        {
            laserBeam.SetActive(false);
        }

        laserSoundPlaying = false;
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
            {
                lifeIcons[i].enabled = i < lives;
            }
        }
    }

    void UpdateLaserUI()
    {
        if (laserAmmoSlider != null)
        {
            laserAmmoSlider.maxValue = maxLaserAmmo;
            laserAmmoSlider.value = laserAmmo;
        }
    }
}
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

    [Header("Laser")]
    public GameObject laserBeam;
    public float laserAmmo = 0f;
    public float maxLaserAmmo = 100f;
    public float laserDrainRate = 25f;
    public KeyCode laserKey = KeyCode.LeftShift;
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
            moveSpeed = GameDataManager.instance.GetMoveSpeed();
            lives = GameDataManager.instance.GetMaxLives();
            shieldHits = GameDataManager.instance.GetShieldHits();
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

        Vector3 moveDirection = new Vector3(moveInputX, 0f, moveInputZ).normalized;
        Vector3 newPos = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

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
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            fireTimer = fireRate;

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(AudioManager.instance.shootSFX);
            }
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
        return Input.GetKey(laserKey) && laserAmmo > 0f;
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
            Debug.Log("Shield hit! Remaining shield: " + shieldHits);

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

        Debug.Log("Player hit! Lives: " + lives);

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
            if (levelManager != null)
            {
                levelManager.GameOver();
            }

            gameObject.SetActive(false);
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
        rb.linearVelocity = Vector3.zero;

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
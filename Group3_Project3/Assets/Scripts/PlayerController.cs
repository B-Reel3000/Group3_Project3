using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float horizontalLimit = 8f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.15f;

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
    private float moveInput;
    private float fireTimer;
    private bool isInvincible = false;
    private bool canControl = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateLivesUI();
    }

    void Update()
    {
        if (!canControl) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (anim != null)
        {
            anim.SetFloat("moveInput", moveInput);
        }

        HandleShooting();
    }

    void FixedUpdate()
    {
        if (!canControl) return;

        Vector3 newPos = rb.position + Vector3.right * moveInput * moveSpeed * Time.fixedDeltaTime;
        newPos.x = Mathf.Clamp(newPos.x, -horizontalLimit, horizontalLimit);
        rb.MovePosition(newPos);
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;

        if (Input.GetButton("Fire1") && fireTimer <= 0f)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            fireTimer = fireRate;
        }
    }

    public void TakeDamage()
    {
        if (isInvincible) return;

        lives--;
        UpdateLivesUI();

        Debug.Log("Player hit! Lives: " + lives);

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
}
using UnityEngine;

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
    public Animator anim; // 👈 THIS is what you’re missing

    private Rigidbody rb;
    private float moveInput;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (anim != null)
        {
            anim.SetFloat("moveInput", moveInput);
        }

        HandleShooting();
    }

    void FixedUpdate()
    {
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
}
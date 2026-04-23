using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Stats")]
    public int maxHealth = 60;
    public int scoreValue = 1000;

    [Header("Shield Phase")]
    public bool shieldActive = true;
    public int laserAttacksBeforeShieldBreak = 4;
    public float shieldDownTime = 8f;
    public GameObject shieldGlowObject;

    [Header("Entrance")]
    public float enterSpeed = 4f;
    public Vector3 targetPosition = new Vector3(0f, 0f, 18f);

    [Header("Idle Spin")]
    public Transform stationVisual;
    public float spinSpeed = 15f;
    public Vector3 spinAxis = new Vector3(0f, 0f, 1f);

    [Header("Hit Pause")]
    public float hitPauseDuration = 0.2f;

    [Header("Damage Flash")]
    public Color damageColor = Color.red;
    public float flashDuration = 0.15f;

    [Header("Audio")]
    public AudioClip bossShootSFX;
    public AudioClip bossLaserSFX;
    public AudioClip bossHitSFX;

    [Header("Bullet Attack")]
    public GameObject bossBulletPrefab;
    public Transform[] bulletFirePoints;
    public float bulletFireRate = 1.5f;

    [Header("Laser Attack")]
    public GameObject[] laserWarningObjects;
    public GameObject[] laserBeamObjects;
    public float laserTelegraphTime = 1.5f;
    public float laserActiveTime = 1.2f;
    public float timeBetweenLaserAttacks = 4f;

    [Header("Effects")]
    public GameObject explosionPrefab;

    [Header("Death Behavior")]
    public bool stopSpinOnDeath = true;
    public Collider[] collidersToDisable;

    [Header("References")]
    public LevelManager levelManager;

    private int currentHealth;
    private bool hasEntered = false;
    private bool isDead = false;
    private bool isLaserSequenceRunning = false;
    private bool isShieldRecovering = false;

    private float bulletTimer;
    private float laserTimer;
    private int laserAttackCount = 0;
    private float hitPauseTimer = 0f;

    private Renderer[] renderers;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    void Start()
    {
        currentHealth = maxHealth;

        if (shieldGlowObject != null)
            shieldGlowObject.SetActive(shieldActive);

        TurnOffAllLaserWarnings();
        TurnOffAllLaserBeams();

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                originalColors[i] = GetRendererColor(renderers[i]);
        }
    }

    void Update()
    {
        if (isDead) return;

        if (!hasEntered)
        {
            EnterArena();
            return;
        }

        SpinVisualOnly();
        HandleBulletAttack();
        HandleLaserAttackTimer();
    }

    void EnterArena()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            enterSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            transform.position = targetPosition;
            hasEntered = true;
        }
    }

    void SpinVisualOnly()
    {
        if (hitPauseTimer > 0f)
        {
            hitPauseTimer -= Time.deltaTime;
            return;
        }

        if (stationVisual != null)
        {
            stationVisual.Rotate(spinAxis.normalized * spinSpeed * Time.deltaTime, Space.Self);
        }
    }

    void HandleBulletAttack()
    {
        bulletTimer -= Time.deltaTime;

        if (bulletTimer <= 0f)
        {
            FireBullets();
            bulletTimer = bulletFireRate;
        }
    }

    void FireBullets()
    {
        if (bossBulletPrefab == null || bulletFirePoints.Length == 0) return;

        for (int i = 0; i < bulletFirePoints.Length; i++)
        {
            Instantiate(bossBulletPrefab, bulletFirePoints[i].position, Quaternion.identity);
        }

        // 🔫 Shoot sound
        if (AudioManager.instance != null && bossShootSFX != null)
        {
            AudioManager.instance.PlaySFX(bossShootSFX);
        }
    }

    void HandleLaserAttackTimer()
    {
        if (isLaserSequenceRunning) return;
        if (isShieldRecovering) return;

        laserTimer -= Time.deltaTime;

        if (laserTimer <= 0f)
        {
            StartCoroutine(FireRandomLaserPattern());
            laserTimer = timeBetweenLaserAttacks;
        }
    }

    IEnumerator FireRandomLaserPattern()
    {
        isLaserSequenceRunning = true;

        int pattern = Random.Range(0, 3);

        if (pattern == 0)
        {
            int lane = Random.Range(0, laserWarningObjects.Length);
            yield return StartCoroutine(FireSingleLaserLane(lane));
        }
        else if (pattern == 1)
        {
            yield return StartCoroutine(FireTwoLaserLanes());
        }
        else
        {
            yield return StartCoroutine(FireAllLaserLanes());
        }

        laserAttackCount++;

        if (shieldActive && laserAttackCount >= laserAttacksBeforeShieldBreak)
        {
            StartCoroutine(ShieldBreakPhase());
        }

        isLaserSequenceRunning = false;
    }

    IEnumerator FireSingleLaserLane(int laneIndex)
    {
        laserWarningObjects[laneIndex].SetActive(true);

        yield return new WaitForSeconds(laserTelegraphTime);

        laserWarningObjects[laneIndex].SetActive(false);
        laserBeamObjects[laneIndex].SetActive(true);

        PlayLaserSound();

        yield return new WaitForSeconds(laserActiveTime);

        laserBeamObjects[laneIndex].SetActive(false);
    }

    IEnumerator FireTwoLaserLanes()
    {
        int a = Random.Range(0, laserWarningObjects.Length);
        int b = (a + 1) % laserWarningObjects.Length;

        laserWarningObjects[a].SetActive(true);
        laserWarningObjects[b].SetActive(true);

        yield return new WaitForSeconds(laserTelegraphTime);

        laserWarningObjects[a].SetActive(false);
        laserWarningObjects[b].SetActive(false);

        laserBeamObjects[a].SetActive(true);
        laserBeamObjects[b].SetActive(true);

        PlayLaserSound();

        yield return new WaitForSeconds(laserActiveTime);

        laserBeamObjects[a].SetActive(false);
        laserBeamObjects[b].SetActive(false);
    }

    IEnumerator FireAllLaserLanes()
    {
        for (int i = 0; i < laserWarningObjects.Length; i++)
            laserWarningObjects[i].SetActive(true);

        yield return new WaitForSeconds(laserTelegraphTime);

        for (int i = 0; i < laserWarningObjects.Length; i++)
        {
            laserWarningObjects[i].SetActive(false);
            laserBeamObjects[i].SetActive(true);
        }

        PlayLaserSound();

        yield return new WaitForSeconds(laserActiveTime);

        for (int i = 0; i < laserBeamObjects.Length; i++)
            laserBeamObjects[i].SetActive(false);
    }

    void PlayLaserSound()
    {
        if (AudioManager.instance != null && bossLaserSFX != null)
        {
            AudioManager.instance.PlaySFX(bossLaserSFX);
        }
    }

    IEnumerator ShieldBreakPhase()
    {
        shieldActive = false;
        isShieldRecovering = true;
        laserAttackCount = 0;

        if (shieldGlowObject != null)
            shieldGlowObject.SetActive(false);

        yield return new WaitForSeconds(shieldDownTime);

        if (!isDead)
        {
            shieldActive = true;

            if (shieldGlowObject != null)
                shieldGlowObject.SetActive(true);
        }

        isShieldRecovering = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || !hasEntered || shieldActive) return;

        currentHealth -= damageAmount;

        hitPauseTimer = hitPauseDuration;

        if (AudioManager.instance != null && bossHitSFX != null)
        {
            AudioManager.instance.PlaySFX(bossHitSFX);
        }

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
            DefeatBoss();
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < renderers.Length; i++)
            SetRendererColor(renderers[i], damageColor);

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
            SetRendererColor(renderers[i], originalColors[i]);

        flashRoutine = null;
    }

    Color GetRendererColor(Renderer rend)
    {
        if (rend.material.HasProperty("_BaseColor"))
            return rend.material.GetColor("_BaseColor");

        return rend.material.color;
    }

    void SetRendererColor(Renderer rend, Color color)
    {
        if (rend.material.HasProperty("_BaseColor"))
            rend.material.SetColor("_BaseColor", color);
        else
            rend.material.color = color;
    }

    void DefeatBoss()
    {
        isDead = true;

        TurnOffAllLaserWarnings();
        TurnOffAllLaserBeams();

        if (shieldGlowObject != null)
            shieldGlowObject.SetActive(false);

        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        DisableBoss();

        if (levelManager != null)
            levelManager.BossDefeated();
    }

    void DisableBoss()
    {
        if (stopSpinOnDeath)
            spinSpeed = 0f;

        foreach (var col in collidersToDisable)
        {
            if (col != null)
                col.enabled = false;
        }
    }

    void TurnOffAllLaserWarnings()
    {
        foreach (var obj in laserWarningObjects)
            if (obj != null) obj.SetActive(false);
    }

    void TurnOffAllLaserBeams()
    {
        foreach (var obj in laserBeamObjects)
            if (obj != null) obj.SetActive(false);
    }
}
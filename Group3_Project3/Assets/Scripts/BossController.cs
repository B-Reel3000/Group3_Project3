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
    public GameObject[] laserWarningBalls;
    public GameObject[] laserBeamObjects;
    public float warningGrowTime = 1.25f;
    public float laserExtendTime = 0.5f;
    public float laserActiveTime = 1.2f;
    public float timeBetweenLaserAttacks = 4f;
    public Vector3 warningStartScale = Vector3.zero;
    public Vector3 warningFullScale = new Vector3(1.5f, 1.5f, 1.5f);

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

    private Vector3[] laserFullScales;

    void Start()
    {
        currentHealth = maxHealth;

        if (shieldGlowObject != null)
            shieldGlowObject.SetActive(shieldActive);

        StoreLaserFullScales();
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

    void StoreLaserFullScales()
    {
        laserFullScales = new Vector3[laserBeamObjects.Length];

        for (int i = 0; i < laserBeamObjects.Length; i++)
        {
            if (laserBeamObjects[i] != null)
                laserFullScales[i] = laserBeamObjects[i].transform.localScale;
        }
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
        if (bossBulletPrefab == null || bulletFirePoints == null || bulletFirePoints.Length == 0) return;

        for (int i = 0; i < bulletFirePoints.Length; i++)
        {
            if (bulletFirePoints[i] != null)
            {
                Instantiate(bossBulletPrefab, bulletFirePoints[i].position, Quaternion.identity);
            }
        }

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

        // 0 = single lane, 1 = double lane.
        // No all-3 pattern, so the player always has a safe lane.
        int pattern = Random.Range(0, 2);

        if (pattern == 0)
        {
            int lane = Random.Range(0, laserWarningBalls.Length);
            yield return StartCoroutine(FireSingleLaserLane(lane));
        }
        else
        {
            int safeLane = Random.Range(0, laserWarningBalls.Length);

            int laneA = (safeLane + 1) % laserWarningBalls.Length;
            int laneB = (safeLane + 2) % laserWarningBalls.Length;

            yield return StartCoroutine(FireTwoLaserLanes(laneA, laneB));
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
        if (!IsValidLaserLane(laneIndex)) yield break;

        yield return StartCoroutine(GrowWarningBall(laneIndex));
        yield return StartCoroutine(ExtendLaser(laneIndex));

        yield return new WaitForSeconds(laserActiveTime);

        TurnOffLaserLane(laneIndex);
    }

    IEnumerator FireTwoLaserLanes(int laneA, int laneB)
    {
        if (!IsValidLaserLane(laneA)) yield break;
        if (!IsValidLaserLane(laneB)) yield break;

        yield return StartCoroutine(GrowTwoWarningBalls(laneA, laneB));
        yield return StartCoroutine(ExtendTwoLasers(laneA, laneB));

        yield return new WaitForSeconds(laserActiveTime);

        TurnOffLaserLane(laneA);
        TurnOffLaserLane(laneB);
    }

    IEnumerator GrowWarningBall(int laneIndex)
    {
        GameObject ball = laserWarningBalls[laneIndex];

        ball.SetActive(true);
        ball.transform.localScale = warningStartScale;

        float timer = 0f;

        while (timer < warningGrowTime)
        {
            timer += Time.deltaTime;
            float t = timer / warningGrowTime;
            ball.transform.localScale = Vector3.Lerp(warningStartScale, warningFullScale, t);
            yield return null;
        }

        ball.transform.localScale = warningFullScale;
    }

    IEnumerator GrowTwoWarningBalls(int laneA, int laneB)
    {
        GameObject ballA = laserWarningBalls[laneA];
        GameObject ballB = laserWarningBalls[laneB];

        ballA.SetActive(true);
        ballB.SetActive(true);

        ballA.transform.localScale = warningStartScale;
        ballB.transform.localScale = warningStartScale;

        float timer = 0f;

        while (timer < warningGrowTime)
        {
            timer += Time.deltaTime;
            float t = timer / warningGrowTime;

            ballA.transform.localScale = Vector3.Lerp(warningStartScale, warningFullScale, t);
            ballB.transform.localScale = Vector3.Lerp(warningStartScale, warningFullScale, t);

            yield return null;
        }

        ballA.transform.localScale = warningFullScale;
        ballB.transform.localScale = warningFullScale;
    }

    IEnumerator ExtendLaser(int laneIndex)
    {
        laserWarningBalls[laneIndex].SetActive(false);

        GameObject beam = laserBeamObjects[laneIndex];
        beam.SetActive(true);

        Vector3 fullScale = laserFullScales[laneIndex];
        Vector3 startScale = new Vector3(fullScale.x, fullScale.y, 0.01f);

        beam.transform.localScale = startScale;

        PlayLaserSound();

        float timer = 0f;

        while (timer < laserExtendTime)
        {
            timer += Time.deltaTime;
            float t = timer / laserExtendTime;
            beam.transform.localScale = Vector3.Lerp(startScale, fullScale, t);
            yield return null;
        }

        beam.transform.localScale = fullScale;
    }

    IEnumerator ExtendTwoLasers(int laneA, int laneB)
    {
        laserWarningBalls[laneA].SetActive(false);
        laserWarningBalls[laneB].SetActive(false);

        GameObject beamA = laserBeamObjects[laneA];
        GameObject beamB = laserBeamObjects[laneB];

        beamA.SetActive(true);
        beamB.SetActive(true);

        Vector3 fullA = laserFullScales[laneA];
        Vector3 fullB = laserFullScales[laneB];

        Vector3 startA = new Vector3(fullA.x, fullA.y, 0.01f);
        Vector3 startB = new Vector3(fullB.x, fullB.y, 0.01f);

        beamA.transform.localScale = startA;
        beamB.transform.localScale = startB;

        PlayLaserSound();

        float timer = 0f;

        while (timer < laserExtendTime)
        {
            timer += Time.deltaTime;
            float t = timer / laserExtendTime;

            beamA.transform.localScale = Vector3.Lerp(startA, fullA, t);
            beamB.transform.localScale = Vector3.Lerp(startB, fullB, t);

            yield return null;
        }

        beamA.transform.localScale = fullA;
        beamB.transform.localScale = fullB;
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
        {
            shieldGlowObject.SetActive(false);
        }

        yield return new WaitForSeconds(shieldDownTime);

        if (!isDead)
        {
            shieldActive = true;

            if (shieldGlowObject != null)
            {
                shieldGlowObject.SetActive(true);
            }
        }

        isShieldRecovering = false;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        if (!hasEntered) return;
        if (shieldActive) return;

        currentHealth -= damageAmount;
        hitPauseTimer = hitPauseDuration;

        if (AudioManager.instance != null && bossHitSFX != null)
        {
            AudioManager.instance.PlaySFX(bossHitSFX);
        }

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            DefeatBoss();
        }
    }

    IEnumerator DamageFlash()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                SetRendererColor(renderers[i], damageColor);
        }

        yield return new WaitForSeconds(flashDuration);

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
                SetRendererColor(renderers[i], originalColors[i]);
        }

        flashRoutine = null;
    }

    Color GetRendererColor(Renderer rend)
    {
        if (rend.material.HasProperty("_BaseColor"))
            return rend.material.GetColor("_BaseColor");

        if (rend.material.HasProperty("_Color"))
            return rend.material.color;

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

    void DefeatBoss()
    {
        isDead = true;
        shieldActive = false;
        isLaserSequenceRunning = false;
        isShieldRecovering = false;

        TurnOffAllLaserWarnings();
        TurnOffAllLaserBeams();

        if (shieldGlowObject != null)
        {
            shieldGlowObject.SetActive(false);
        }

        if (GameDataManager.instance != null)
        {
            GameDataManager.instance.AddScore(scoreValue);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.explosionSFX);
        }

        DisableBoss();

        if (levelManager != null)
        {
            levelManager.BossDefeated();
        }
    }

    void DisableBoss()
    {
        if (stopSpinOnDeath)
        {
            spinSpeed = 0f;
        }

        if (collidersToDisable == null) return;

        for (int i = 0; i < collidersToDisable.Length; i++)
        {
            if (collidersToDisable[i] != null)
            {
                collidersToDisable[i].enabled = false;
            }
        }
    }

    void TurnOffLaserLane(int laneIndex)
    {
        if (!IsValidLaserLane(laneIndex)) return;

        laserWarningBalls[laneIndex].SetActive(false);
        laserBeamObjects[laneIndex].SetActive(false);
        laserBeamObjects[laneIndex].transform.localScale = laserFullScales[laneIndex];
    }

    void TurnOffAllLaserWarnings()
    {
        if (laserWarningBalls == null) return;

        for (int i = 0; i < laserWarningBalls.Length; i++)
        {
            if (laserWarningBalls[i] != null)
            {
                laserWarningBalls[i].SetActive(false);
            }
        }
    }

    void TurnOffAllLaserBeams()
    {
        if (laserBeamObjects == null) return;

        for (int i = 0; i < laserBeamObjects.Length; i++)
        {
            if (laserBeamObjects[i] != null)
            {
                laserBeamObjects[i].SetActive(false);

                if (laserFullScales != null && i < laserFullScales.Length)
                {
                    laserBeamObjects[i].transform.localScale = laserFullScales[i];
                }
            }
        }
    }

    bool IsValidLaserLane(int laneIndex)
    {
        if (laneIndex < 0) return false;
        if (laneIndex >= laserWarningBalls.Length) return false;
        if (laneIndex >= laserBeamObjects.Length) return false;
        if (laserWarningBalls[laneIndex] == null) return false;
        if (laserBeamObjects[laneIndex] == null) return false;

        return true;
    }
}
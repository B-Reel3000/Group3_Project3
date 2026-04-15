using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelTime = 60f;
    public AsteroidSpawner asteroidSpawner;
    public PowerUpSpawner powerUpSpawner;
    public ScrapSpawner scrapSpawner;
    public EnemySpawner enemySpawner;
    public PlayerController player;

    [Header("Travel UI")]
    public Slider travelSlider;

    [Header("Scene Flow")]
    public string nextSceneName = "Shop";
    public string startLevelText = "Level One";
    public string endLevelText = "Destination Reached";

    [Header("UI")]
    public LevelTextUI levelTextUI;
    public GameObject gameOverPanel;

    private float timer;
    private bool levelEnded = false;
    private float travelSpeedMultiplier = 1f;

    void Start()
    {
        timer = 0f;

        if (GameDataManager.instance != null)
        {
            travelSpeedMultiplier = GameDataManager.instance.GetTravelSpeedMultiplier();
        }

        if (travelSlider != null)
        {
            travelSlider.minValue = 0f;
            travelSlider.maxValue = 1f;
            travelSlider.value = 0f;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        StartCoroutine(ShowStartText());
    }

    IEnumerator ShowStartText()
    {
        yield return new WaitForSeconds(0.5f);

        if (levelTextUI != null)
        {
            levelTextUI.ShowMessage(startLevelText);
        }
    }

    void Update()
    {
        if (levelEnded) return;

        timer += Time.deltaTime * travelSpeedMultiplier;

        if (travelSlider != null)
        {
            travelSlider.value = timer / levelTime;
        }

        if (timer >= levelTime)
        {
            WinLevel();
        }
    }

    void WinLevel()
    {
        levelEnded = true;
        timer = levelTime;

        if (travelSlider != null)
        {
            travelSlider.value = 1f;
        }

        StopGameplay();

        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        if (levelTextUI != null)
        {
            levelTextUI.ShowMessage(endLevelText);
        }

        yield return new WaitForSeconds(2.5f);

        if (FadeManager.instance != null)
        {
            FadeManager.instance.LoadSceneWithFade(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void GameOver()
    {
        if (levelEnded) return;

        levelEnded = true;
        StopGameplay();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    void StopGameplay()
    {
        if (asteroidSpawner != null) asteroidSpawner.enabled = false;
        if (powerUpSpawner != null) powerUpSpawner.enabled = false;
        if (scrapSpawner != null) scrapSpawner.enabled = false;
        if (enemySpawner != null) enemySpawner.enabled = false;

        if (player != null)
        {
            player.DisableControl();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("End UI")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

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

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
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

        if (asteroidSpawner != null) asteroidSpawner.enabled = false;
        if (powerUpSpawner != null) powerUpSpawner.enabled = false;
        if (scrapSpawner != null) scrapSpawner.enabled = false;
        if (enemySpawner != null) enemySpawner.enabled = false;

        if (player != null)
        {
            player.DisableControl();
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }

    public void GameOver()
    {
        if (levelEnded) return;

        levelEnded = true;

        if (asteroidSpawner != null) asteroidSpawner.enabled = false;
        if (powerUpSpawner != null) powerUpSpawner.enabled = false;
        if (scrapSpawner != null) scrapSpawner.enabled = false;
        if (enemySpawner != null) enemySpawner.enabled = false;

        if (player != null)
        {
            player.DisableControl();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
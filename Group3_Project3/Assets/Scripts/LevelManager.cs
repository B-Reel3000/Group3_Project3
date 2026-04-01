using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    public float levelTime = 60f;
    public AsteroidSpawner asteroidSpawner;
    public PlayerController player;

    [Header("Travel UI")]
    public Slider travelSlider;

    [Header("End UI")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

    private float timer;
    private bool levelEnded = false;

    void Start()
    {
        timer = 0f;

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

        timer += Time.deltaTime;

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

        Debug.Log("Level Complete!");

        if (asteroidSpawner != null)
        {
            asteroidSpawner.enabled = false;
        }

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

        Debug.Log("Game Over");

        if (asteroidSpawner != null)
        {
            asteroidSpawner.enabled = false;
        }

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
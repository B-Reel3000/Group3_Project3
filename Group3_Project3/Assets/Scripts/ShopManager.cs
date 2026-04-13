using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text infoText;

    [Header("Upgrade Costs")]
    public int maxLivesCost = 100;
    public int shieldCost = 150;
    public int speedCost = 125;
    public int spreadCost = 200;

    [Header("Next Scene")]
    public string nextLevelSceneName = "Level2";

    void Start()
    {
        if (infoText != null)
        {
            infoText.text = "";
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null && GameDataManager.instance != null)
        {
            scoreText.text = "Score: " + GameDataManager.instance.score;
        }
    }

    public void BuyMaxLives()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.SpendScore(maxLivesCost))
        {
            GameDataManager.instance.maxLivesLevel++;
            ShowMessage("Max Lives upgraded!");
        }
        else
        {
            ShowMessage("Not enough score!");
        }
    }

    public void BuyShield()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.SpendScore(shieldCost))
        {
            GameDataManager.instance.shieldLevel++;
            ShowMessage("Shield upgraded!");
        }
        else
        {
            ShowMessage("Not enough score!");
        }
    }

    public void BuySpeed()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.SpendScore(speedCost))
        {
            GameDataManager.instance.speedLevel++;
            ShowMessage("Speed upgraded!");
        }
        else
        {
            ShowMessage("Not enough score!");
        }
    }

    public void BuySpread()
    {
        if (GameDataManager.instance != null && GameDataManager.instance.SpendScore(spreadCost))
        {
            GameDataManager.instance.spreadLevel++;
            ShowMessage("Spread Fire upgraded!");
        }
        else
        {
            ShowMessage("Not enough score!");
        }
    }

    public void ContinueToNextLevel()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    void ShowMessage(string message)
    {
        if (infoText != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowMessageRoutine(message));
        }

        Debug.Log(message);
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        infoText.text = message;

        if (message.Contains("Not enough"))
        {
            infoText.color = Color.red;
        }
        else
        {
            infoText.color = Color.green;
        }

        yield return new WaitForSeconds(2f);

        infoText.text = "";
    }
}
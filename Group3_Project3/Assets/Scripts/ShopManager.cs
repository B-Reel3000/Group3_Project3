using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;

    [Header("Costs")]
    public int maxLivesCost = 100;
    public int shieldCost = 150;
    public int speedCost = 150;
    public int spreadCost = 200;

    [Header("Next Scene")]
    public string nextLevelSceneName = "Level2";

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

    public void BuyMaxLivesUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (!GameDataManager.instance.CanBuyMaxLives())
        {
            Debug.Log("Max Lives is already at max.");
            return;
        }

        if (GameDataManager.instance.SpendScore(maxLivesCost))
        {
            GameDataManager.instance.maxLivesPurchases++;
            Debug.Log("Bought Max Lives upgrade.");
        }
        else
        {
            Debug.Log("Not enough score for Max Lives.");
        }
    }

    public void BuyShieldUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasShieldUpgrade)
        {
            Debug.Log("Shield already purchased.");
            return;
        }

        if (GameDataManager.instance.SpendScore(shieldCost))
        {
            GameDataManager.instance.hasShieldUpgrade = true;
            Debug.Log("Bought Shield upgrade.");
        }
        else
        {
            Debug.Log("Not enough score for Shield.");
        }
    }

    public void BuySpeedUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasSpeedUpgrade)
        {
            Debug.Log("Speed already purchased.");
            return;
        }

        if (GameDataManager.instance.SpendScore(speedCost))
        {
            GameDataManager.instance.hasSpeedUpgrade = true;
            Debug.Log("Bought Speed upgrade.");
        }
        else
        {
            Debug.Log("Not enough score for Speed.");
        }
    }

    public void BuySpreadUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasSpreadUpgrade)
        {
            Debug.Log("Spread already purchased.");
            return;
        }

        if (GameDataManager.instance.SpendScore(spreadCost))
        {
            GameDataManager.instance.hasSpreadUpgrade = true;
            Debug.Log("Bought Spread upgrade.");
        }
        else
        {
            Debug.Log("Not enough score for Spread.");
        }
    }

    public void ContinueToNextLevel()
    {
        SceneManager.LoadScene(nextLevelSceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
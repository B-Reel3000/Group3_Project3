using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text infoText;

    [Header("Costs")]
    public int maxLivesCost = 100;
    public int shieldCost = 150;
    public int speedCost = 150;
    public int spreadCost = 200;

    [Header("Next Scene")]
    public string nextLevelSceneName = "Level2Cinematic";

    void Start()
    {
        if (infoText != null)
        {
            infoText.text = "";
        }
    }

    void Update()
    {
        if (scoreText != null && GameDataManager.instance != null)
        {
            scoreText.text = "Score: " + GameDataManager.instance.score;
        }
    }

    // ---------- PURCHASE METHODS ----------

    public void BuyMaxLivesUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (!GameDataManager.instance.CanBuyMaxLives())
        {
            ShowInfo("Max lives already upgraded");
            return;
        }

        if (GameDataManager.instance.SpendScore(maxLivesCost))
        {
            GameDataManager.instance.maxLivesPurchases++;
            ShowInfo("Max lives increased!");
        }
        else
        {
            ShowInfo("Not enough score");
        }
    }

    public void BuyShieldUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasShieldUpgrade)
        {
            ShowInfo("Shield already purchased");
            return;
        }

        if (GameDataManager.instance.SpendScore(shieldCost))
        {
            GameDataManager.instance.hasShieldUpgrade = true;
            ShowInfo("Shield acquired!");
        }
        else
        {
            ShowInfo("Not enough score");
        }
    }

    public void BuySpeedUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasSpeedUpgrade)
        {
            ShowInfo("Speed already upgraded");
            return;
        }

        if (GameDataManager.instance.SpendScore(speedCost))
        {
            GameDataManager.instance.hasSpeedUpgrade = true;
            ShowInfo("Speed increased!");
        }
        else
        {
            ShowInfo("Not enough score");
        }
    }

    public void BuySpreadUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasSpreadUpgrade)
        {
            ShowInfo("Spread already purchased");
            return;
        }

        if (GameDataManager.instance.SpendScore(spreadCost))
        {
            GameDataManager.instance.hasSpreadUpgrade = true;
            ShowInfo("Spread shot unlocked!");
        }
        else
        {
            ShowInfo("Not enough score");
        }
    }

    // ---------- SCENE TRANSITIONS ----------

    public void ContinueToNextLevel()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.LoadSceneWithFade(nextLevelSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
    }

    public void ReturnToMainMenu()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.LoadSceneWithFade("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // ---------- INFO TEXT ----------

    void ShowInfo(string message)
    {
        if (infoText == null) return;

        StopAllCoroutines();
        StartCoroutine(ShowInfoRoutine(message));
    }

    IEnumerator ShowInfoRoutine(string message)
    {
        infoText.text = message;

        yield return new WaitForSeconds(2f);

        infoText.text = "";
    }
}
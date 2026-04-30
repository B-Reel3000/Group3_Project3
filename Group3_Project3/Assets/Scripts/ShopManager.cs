using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text infoText;

    [Header("Buttons")]
    public Button maxLivesButton;
    public Button shieldButton;
    public Button speedButton;
    public Button spreadButton;

    [Header("Button Colors")]
    public Color availableColor = Color.white;
    public Color unavailableColor = Color.red;

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

        UpdateShopUI();
    }

    void Update()
    {
        UpdateShopUI();
    }

    void UpdateShopUI()
    {
        if (GameDataManager.instance == null) return;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + GameDataManager.instance.score;
        }

        UpdateButtonVisual(maxLivesButton, GameDataManager.instance.CanBuyMaxLives());
        UpdateButtonVisual(shieldButton, !GameDataManager.instance.hasShieldUpgrade);
        UpdateButtonVisual(speedButton, !GameDataManager.instance.hasSpeedUpgrade);
        UpdateButtonVisual(spreadButton, !GameDataManager.instance.hasSpreadUpgrade);
    }

    void UpdateButtonVisual(Button button, bool canBuy)
    {
        if (button == null) return;

        Image buttonImage = button.GetComponent<Image>();

        if (buttonImage != null)
        {
            buttonImage.color = canBuy ? availableColor : unavailableColor;
        }

        button.interactable = canBuy;
    }

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

        UpdateShopUI();
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

        UpdateShopUI();
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

        UpdateShopUI();
    }

    public void BuySpreadUpgrade()
    {
        if (GameDataManager.instance == null) return;

        if (GameDataManager.instance.hasSpreadUpgrade)
        {
            ShowInfo("Triple shot already purchased");
            return;
        }

        if (GameDataManager.instance.SpendScore(spreadCost))
        {
            GameDataManager.instance.hasSpreadUpgrade = true;
            ShowInfo("Triple shot unlocked!");
        }
        else
        {
            ShowInfo("Not enough score");
        }

        UpdateShopUI();
    }

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
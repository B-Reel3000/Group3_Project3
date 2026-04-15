using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [Header("Score")]
    public int score = 0;

    [Header("Lives Upgrade")]
    public int maxLivesPurchases = 0;
    public int maxLivesPurchaseLimit = 2;

    [Header("One-Time Upgrades")]
    public bool hasShieldUpgrade = false;
    public bool hasSpeedUpgrade = false;
    public bool hasSpreadUpgrade = false;

    [Header("Base Values")]
    public int baseLives = 3;
    public int shieldHitsWhenUnlocked = 5;
    public float baseTravelSpeedMultiplier = 1f;
    public float upgradedTravelSpeedMultiplier = 1.35f;
    public int baseSpreadBulletCount = 3;
    public int upgradedSpreadBulletCount = 5;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (score < 0)
        {
            score = 0;
        }
    }

    public bool SpendScore(int amount)
    {
        if (score >= amount)
        {
            score -= amount;
            return true;
        }

        return false;
    }

    public int GetMaxLives()
    {
        return baseLives + maxLivesPurchases;
    }

    public int GetShieldHits()
    {
        if (hasShieldUpgrade)
        {
            return shieldHitsWhenUnlocked;
        }

        return 0;
    }

    public float GetTravelSpeedMultiplier()
    {
        if (hasSpeedUpgrade)
        {
            return upgradedTravelSpeedMultiplier;
        }

        return baseTravelSpeedMultiplier;
    }

    public int GetSpreadBulletCount()
    {
        if (hasSpreadUpgrade)
        {
            return upgradedSpreadBulletCount;
        }

        return 1;
    }

    public bool CanBuyMaxLives()
    {
        return maxLivesPurchases < maxLivesPurchaseLimit;
    }

    public void ResetAllData()
    {
        score = 0;
        maxLivesPurchases = 0;
        hasShieldUpgrade = false;
        hasSpeedUpgrade = false;
        hasSpreadUpgrade = false;
    }
}
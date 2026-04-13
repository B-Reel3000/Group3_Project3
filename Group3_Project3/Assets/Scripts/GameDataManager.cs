using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;

    [Header("Score")]
    public int score = 0;

    [Header("Upgrade Levels")]
    public int maxLivesLevel = 0;
    public int shieldLevel = 0;
    public int speedLevel = 0;
    public int spreadLevel = 0;

    [Header("Base Values")]
    public int baseLives = 3;
    public int baseShieldHits = 5;
    public float baseMoveSpeed = 10f;

    public float baseSpreadCooldown = 5f;
    public int baseSpreadBulletCount = 3;

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
        if (score < 0) score = 0;
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
        return baseLives + maxLivesLevel;
    }

    public int GetShieldHits()
    {
        return baseShieldHits + (shieldLevel * 2);
    }

    public float GetMoveSpeed()
    {
        return baseMoveSpeed + (speedLevel * 2f);
    }

    public float GetSpreadCooldown()
    {
        return Mathf.Max(1f, baseSpreadCooldown - (spreadLevel * 0.5f));
    }

    public int GetSpreadBulletCount()
    {
        return baseSpreadBulletCount + spreadLevel;
    }
}
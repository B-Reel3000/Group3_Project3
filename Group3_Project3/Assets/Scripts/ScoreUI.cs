using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void Update()
    {
        if (scoreText != null && GameDataManager.instance != null)
        {
            scoreText.text = "Score: " + GameDataManager.instance.score;
        }
    }
}

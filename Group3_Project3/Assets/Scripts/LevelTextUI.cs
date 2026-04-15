using UnityEngine;
using TMPro;
using System.Collections;

public class LevelTextUI : MonoBehaviour
{
    public TMP_Text messageText;
    public float showTime = 2f;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        StartCoroutine(ShowMessageRoutine(message));
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(showTime);

        messageText.gameObject.SetActive(false);
    }
}
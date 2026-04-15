using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;

public class CinematicSceneController : MonoBehaviour
{
    public PlayableDirector director;
    public string nextSceneName = "Level2";
    public float extraWaitTime = 0.5f;

    void Start()
    {
        if (director != null)
        {
            director.Play();
            StartCoroutine(WaitForCinematic());
        }
    }

    IEnumerator WaitForCinematic()
    {
        yield return new WaitForSeconds((float)director.duration + extraWaitTime);

        if (FadeManager.instance != null)
        {
            FadeManager.instance.LoadSceneWithFade(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
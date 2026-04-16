using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System.Collections;

public class MainMenuPlaySequence : MonoBehaviour
{
    [Header("Timeline")]
    public PlayableDirector director;
    public float extraDelayAfterTimeline = 0.2f;

    [Header("UI")]
    public GameObject mainMenuButtons;
    public GameObject backButton;

    [Header("Scene Flow")]
    public string nextSceneName = "Level1";

    private bool isStarting = false;
    private bool hasLoadedNextScene = false;

    void OnEnable()
    {
        if (director != null)
        {
            director.stopped += OnTimelineFinished;
        }
    }

    void OnDisable()
    {
        if (director != null)
        {
            director.stopped -= OnTimelineFinished;
        }
    }

    public void PlayGame()
    {
        if (isStarting) return;

        isStarting = true;
        hasLoadedNextScene = false;

        if (mainMenuButtons != null)
        {
            mainMenuButtons.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.SetActive(false);
        }

        if (director != null)
        {
            director.Play();
            StartCoroutine(FallbackLoadAfterTimeline());
        }
        else
        {
            LoadNextScene();
        }
    }

    void OnTimelineFinished(PlayableDirector stoppedDirector)
    {
        if (stoppedDirector != director) return;

        LoadNextScene();
    }

    IEnumerator FallbackLoadAfterTimeline()
    {
        if (director == null) yield break;

        double waitTime = director.duration + extraDelayAfterTimeline;
        yield return new WaitForSeconds((float)waitTime);

        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (hasLoadedNextScene) return;
        hasLoadedNextScene = true;

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
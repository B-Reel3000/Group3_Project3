using UnityEngine;
using Cinemachine;
using System.Collections;

public class MenuCameraManager : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera mainMenuCam;
    public CinemachineVirtualCamera instructionsCam;
    public CinemachineVirtualCamera creditsCam;

    [Header("Priorities")]
    public int activePriority = 20;
    public int inactivePriority = 10;

    [Header("Reveal Timing")]
    public float cameraBlendTime = 1f;

    [Header("Instruction Objects")]
    public GameObject[] instructionObjects;

    [Header("Credits Objects")]
    public GameObject[] creditsObjects;

    [Header("UI Groups")]
    public GameObject mainMenuButtons;
    public GameObject backButton;

    void Start()
    {
        HideAllSpecialObjects();
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        StopAllCoroutines();
        HideAllSpecialObjects();
        SetAllInactive();

        if (mainMenuCam != null)
        {
            mainMenuCam.Priority = activePriority;
        }

        if (mainMenuButtons != null)
        {
            mainMenuButtons.SetActive(true);
        }

        if (backButton != null)
        {
            backButton.SetActive(false);
        }
    }

    public void ShowInstructions()
    {
        StopAllCoroutines();
        HideAllSpecialObjects();
        SetAllInactive();

        if (instructionsCam != null)
        {
            instructionsCam.Priority = activePriority;
        }

        if (mainMenuButtons != null)
        {
            mainMenuButtons.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.SetActive(true);
        }

        StartCoroutine(RevealAfterBlend(instructionObjects));
    }

    public void ShowCredits()
    {
        StopAllCoroutines();
        HideAllSpecialObjects();
        SetAllInactive();

        if (creditsCam != null)
        {
            creditsCam.Priority = activePriority;
        }

        if (mainMenuButtons != null)
        {
            mainMenuButtons.SetActive(false);
        }

        if (backButton != null)
        {
            backButton.SetActive(true);
        }

        StartCoroutine(RevealAfterBlend(creditsObjects));
    }

    IEnumerator RevealAfterBlend(GameObject[] objectsToShow)
    {
        yield return new WaitForSeconds(cameraBlendTime);

        foreach (GameObject obj in objectsToShow)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    void HideAllSpecialObjects()
    {
        foreach (GameObject obj in instructionObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        foreach (GameObject obj in creditsObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    void SetAllInactive()
    {
        if (mainMenuCam != null)
        {
            mainMenuCam.Priority = inactivePriority;
        }

        if (instructionsCam != null)
        {
            instructionsCam.Priority = inactivePriority;
        }

        if (creditsCam != null)
        {
            creditsCam.Priority = inactivePriority;
        }
    }
}
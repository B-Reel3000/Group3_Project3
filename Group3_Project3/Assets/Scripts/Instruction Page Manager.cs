using UnityEngine;

public class InstructionPageManager : MonoBehaviour
{
    [Header("Instruction Pages")]
    public GameObject[] pages;

    private int currentPage = 0;

    void Start()
    {
        ShowPage(0);
    }

    public void ShowNextPage()
    {
        if (pages == null || pages.Length == 0) return;

        currentPage++;

        if (currentPage >= pages.Length)
        {
            currentPage = pages.Length - 1;
        }

        ShowPage(currentPage);
    }

    public void ShowPreviousPage()
    {
        if (pages == null || pages.Length == 0) return;

        currentPage--;

        if (currentPage < 0)
        {
            currentPage = 0;
        }

        ShowPage(currentPage);
    }

    public void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0) return;

        if (pageIndex < 0 || pageIndex >= pages.Length) return;

        currentPage = pageIndex;

        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == currentPage);
            }
        }
    }

    public void ResetToFirstPage()
    {
        ShowPage(0);
    }
}
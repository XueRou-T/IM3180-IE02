using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class HelpPanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panels;

    [Header("Page Display")]
    public TMP_Text pageTitle;
    public TMP_Text pageDesc;
    public Image pageImage;

    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton;
    public Button closeButton;

    [Header("Help Pages")]
    public List<HelpPage> helpPages = new List<HelpPage>();

    private int currentPage = 0;

    void Start()
    {
        panels.SetActive(false);
    }

    public void OpenHelp()
    {
        currentPage = 0;

        panels.SetActive(true);

        UpdatePage();
    }

    void UpdatePage()
    {
        if(helpPages.Count == 0)
        {
            pageTitle.text = "";
            pageDesc.text = "";
            pageImage.sprite = null;

            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);

            return;
        }

        HelpPage helpPage = helpPages[currentPage];

        pageTitle.text = helpPage.title;
        pageDesc.text = helpPage.description;
        pageImage.sprite = helpPage.image;

        nextButton.gameObject.SetActive(currentPage < helpPages.Count - 1);
        prevButton.gameObject.SetActive(currentPage > 0);

        return;
    }

    public void NextPage()
    {
        if (currentPage >= helpPages.Count - 1)
            return;

        currentPage++;

        UpdatePage();
    }

    public void PrevPage()
    {
        if (currentPage <=0)
            return;

        currentPage--;

        UpdatePage();
    }

    public void CloseHelp()
    {
        panels.SetActive(false);
    }
}

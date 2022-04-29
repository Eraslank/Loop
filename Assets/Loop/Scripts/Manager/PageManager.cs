using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PageManager : MonoBehaviour
{
    public static PageManager instance;
    public List<Page> pages;
    Page currentPage;
    private Sequence sequence;
    public void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        var pageToOpen = GetPage((PageName)PlayerPrefs.GetInt("CurrentPage"));
        PlayerPrefs.DeleteKey("CurrentPage");
        SetPage(GetPage(PageName.MainMenu), false);

        if (pageToOpen != null)
        {
            ChangePage(pageToOpen);
        }
        else
            ChangePage(PageName.MainMenu);
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }

    public void ChangePage(Page page)
    {
        if (currentPage != null && currentPage == page)
            return;
        if (currentPage != null)
            SetPage(currentPage, false);
        currentPage = page;
        SetPage(currentPage, true);

        Application.targetFrameRate = 60;
    }
    public void ChangePage(PageName pageName ,bool forcePageChange = false)
    {
        if (currentPage != null && currentPage.pageName == pageName && !forcePageChange)
            return;
        Page page = GetPage(pageName);
        if (currentPage != null)
            SetPage(currentPage, false);
        currentPage = page;
        SetPage(currentPage, true);

        Application.targetFrameRate = 60;
    }
    void SetPage(Page page, bool showPage)
    {
        if (showPage)
        {
            IConfigurablePage iCP = page.GetComponent<IConfigurablePage>();
            if (iCP != null)
                iCP.ConfigurePage();
        }
        PageAnimation(page,showPage);
    }

    public Page GetPage(PageName page)
    {
        return pages.Select(p => p).Where(p => p.pageName == page).First();
    }
    private void PageAnimation(Page page,bool showPage)
    {
        page.GetComponent<CanvasGroup>().DOFade(0f, .4f).OnComplete(() =>
        {
            page.GetComponent<CanvasGroup>().DOFade(1f, .4f);
            page.isVisible(showPage);
        });
    }
}

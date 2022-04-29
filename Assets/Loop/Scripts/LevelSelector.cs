using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelSelector : MonoBehaviour
{
    private int unlockedLevel;
    private Levels[] levelButton;
    private int totalUnlockedLevel = 0;
    private int page = 0;
    private int pageItem = 25;
    public GameObject nextButton, backButton;
    private CanvasGroup canvasGroup;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        levelButton = GetComponentsInChildren<Levels>();
        unlockedLevel = PlayerPrefs.GetInt("LastCompleted") + 1;
        totalUnlockedLevel = (Mathf.CeilToInt((unlockedLevel) / (float)pageItem)) * pageItem;
        page = totalUnlockedLevel / pageItem;
        Refresh();
    }
    public void ClickNext()
    {
        ButtonClick(nextButton, 1);
    }
    public void ClickBack()
    {
        ButtonClick(backButton, -1);
    }
    private void ButtonClick(GameObject button,int number)
    {
        button.GetComponent<Button>().interactable = false;
        
        canvasGroup.DOFade(0f, .4f).OnComplete(() =>
        {
            button.GetComponent<Button>().interactable = true;
            page += number;
            Refresh();
            canvasGroup.DOFade(1f, .4f);
        }).Play();
    }
    public void Refresh()
    {
        int index = page * pageItem;
        for (int i = 0; i < levelButton.Length; i++)
        {
            int level = index - i;
            if (level <= totalUnlockedLevel)
            {
                levelButton[(level - 1) % 25].gameObject.SetActive(true);
                levelButton[(level - 1) % 25].Setup(level, level <= unlockedLevel);
            }
            else
                levelButton[(level - 1) % 25].gameObject.SetActive(false);
        }
        CheckButton();
    }
    private void CheckButton()
    {
        backButton.SetActive(page > 1 );
        nextButton.SetActive(page < totalUnlockedLevel / pageItem);
    }
}

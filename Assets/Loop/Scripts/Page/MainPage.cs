using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPage : Page, IConfigurablePage
{
    [SerializeField] GameObject levelsButton;

    public void ConfigurePage(bool isFirstTime = false)
    {
        int lastCompleted = LevelManager.Instance.UserLevel.lastCompleted;

        levelsButton.SetActive(lastCompleted != 0);
    }

    public void OnPlayButtonClick()
    {
        LevelManager.Instance.UserLevel.currentLevelId = LevelManager.Instance.UserLevel.lastCompleted + 1;
        SceneManager.LoadScene(1);
    }
}

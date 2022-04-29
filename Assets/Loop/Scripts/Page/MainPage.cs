using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : Page, IConfigurablePage
{
    [SerializeField] GameObject levelsButton;

    public void ConfigurePage(bool isFirstTime = false)
    {
        if (!PlayerPrefs.HasKey("LastCompleted"))
            PlayerPrefs.SetInt("LastCompleted", 0);

        int lastCompleted = PlayerPrefs.GetInt("LastCompleted");

        levelsButton.SetActive(lastCompleted != 0);
    }

    public void OnPlayButtonClick()
    {
        PlayerPrefs.SetInt("LevelId", PlayerPrefs.GetInt("LastCompleted") + 1);
        SceneChangeManager.Instance.ChangeScene("MainScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPage : Page
{
    [SerializeField] InputField levelInputField;

    public void OnPlayButtonClick()
    {
        int inputLevel = int.Parse(levelInputField.text);
        inputLevel = Mathf.Abs(inputLevel);

        levelInputField.text = inputLevel.ToString();

        if (inputLevel == 0 || inputLevel > LevelManager.Instance.UserLevel.lastCompleted + 1)
            return;

        LevelManager.Instance.UserLevel.currentLevelId = inputLevel;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}

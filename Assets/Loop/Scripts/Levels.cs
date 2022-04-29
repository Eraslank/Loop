using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Levels : MonoBehaviour
{
    public Sprite lockSprite;
    public Text levelText;
    private int level = 0;
    private Button button;
    private Image image;
    private void OnEnable()
    {
        button = GetComponent<Button>();
        image = transform.GetChild(1).GetComponent<Image>();
    }
    public void Setup(int level, bool isUnlock)
    {
        this.level = level;
        levelText.text = level.ToString();
        if (isUnlock)
        {
            LockLevel(isUnlock, true, true);
        }
        else
        {
            LockLevel(isUnlock, false, false);
        }
    }
    private void LockLevel(bool isUnlock, bool buttonVisibility, bool text)
    {
        image.gameObject.SetActive(!isUnlock);
        button.enabled = buttonVisibility;
        levelText.gameObject.SetActive(text);
    }
    public void OnClick()
    {
        PlayerPrefs.SetInt("LevelId",int.Parse(levelText.text));
        SceneChangeManager.Instance.ChangeScene("MainScene");
    }

}

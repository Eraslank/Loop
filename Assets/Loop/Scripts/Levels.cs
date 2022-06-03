using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Levels : MonoBehaviour
{
    public Text levelText;
    private int level = 0;
    [SerializeField] private Button button;
    [SerializeField] private Image lockImage;
    [SerializeField] private Image starCountImage;
    public void Setup(int level, bool isUnlock)
    {
        this.level = level;
        levelText.text = level.ToString();

        var stars = LevelManager.Instance.UserLevel.stars;
        starCountImage.fillAmount = 0;
        if(stars.ContainsKey(level))
            starCountImage.fillAmount = stars[level] == 3 ? 1 : stars[level] * .33f;

        LockLevel(isUnlock);
    }
    private void LockLevel(bool isUnlock)
    {
        lockImage.gameObject.SetActive(!isUnlock);
        button.enabled = isUnlock;
        levelText.gameObject.SetActive(isUnlock);
    }
    public void OnClick()
    {
        LevelManager.Instance.UserLevel.currentLevelId = int.Parse(levelText.text);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ButtonAnimation : MonoBehaviour
{
    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => 
        {
            GetComponent<Transform>().DOScale(1.2f, .1f).OnComplete(() =>
            {
                GetComponent<Transform>().DOScale(1, .1f);
            });
        });
    }
}

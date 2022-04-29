using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Node : MonoBehaviour, IPointerClickHandler
{
    public NodeData nodeData;



    public Dictionary<EDirection, Node> neighbors;
    public List<EDirection> currentTips;
    public EDirection rotateState = EDirection.Up;
    public int row, column;

    private void Start()
    {
        GameManager.Instance.SetNeighbors(this, out _);
    }
    public void Initialize(NodeData nodeData)
    {
        this.nodeData = nodeData;
        GetComponent<Image>().sprite = nodeData.image;
        transform.rotation = Quaternion.Euler(0, 0, -90 * (int)(rotateState = (EDirection)Random.Range(0, (int)EDirection.Count)));
        currentTips = nodeData.GetRotatedTips(rotateState);
    }

    bool canRotate = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canRotate)
            return;
        canRotate = false;
        transform.DORotate(new Vector3(0, 0, transform.eulerAngles.z - 90), .25f, RotateMode.FastBeyond360)
        .SetEase(Ease.InOutCirc)
        .OnComplete(() => 
        {
            canRotate = true;
            rotateState = rotateState.Next();
            currentTips = nodeData.GetRotatedTips(rotateState);
            GameManager.Instance.CheckLevelEnd();
        });
    }

    public bool IsDone()
    {
        foreach (var tip in currentTips)
        {
            if (neighbors[tip] == null || !neighbors[tip].currentTips.Contains(tip.Inverse()))
            {
                return false;
            }
        }
        return true;
    }
}

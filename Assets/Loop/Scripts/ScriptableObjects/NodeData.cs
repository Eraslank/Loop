using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Node Data", menuName = "Node Data")]
public class NodeData : ScriptableObject
{
    public bool straight = false;
    public List<EDirection> openTips;
    public Sprite image;

    public List<EDirection> GetRotatedTips(EDirection rotateState)
    {
        if (openTips.Count == 4)
            return openTips;
        List<EDirection> tips = new List<EDirection>();
        foreach(var tip in openTips)
        {
            var newTip = tip;
            for (int i = 0; i < (int)rotateState; i++)
            {
                newTip = newTip.Next();
            }
            tips.Add(newTip);
        }
        return tips;
    }
}

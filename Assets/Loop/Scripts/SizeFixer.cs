using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SizeFixer : MonoBehaviour
{
    RectTransform parentRT;
    RectTransform rT;
    IEnumerator Start()
    {
        parentRT = transform.parent.GetRectTransform();
        rT = transform.GetRectTransform();

        while(true)
        {
            while (parentRT.rect.width == rT.rect.width)
                yield return null;
            rT.sizeDelta = new Vector2(parentRT.rect.width, rT.rect.height);
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(SizeFixer))]
public class SizeFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SizeFixer sF = (SizeFixer)target;
        if (GUILayout.Button("Fix"))
        {
            sF.GetRectTransform().sizeDelta = new Vector2(sF.transform.parent.GetRectTransform().rect.width
                , sF.GetRectTransform().rect.height);
        }
    }
}

#endif
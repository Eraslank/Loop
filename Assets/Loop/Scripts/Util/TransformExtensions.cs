using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform transform, bool immediate = false)
    {
        foreach (Transform t in transform)
        {
            if (immediate)
                Object.DestroyImmediate(t.gameObject);
            else
                Object.Destroy(t.gameObject);
        }
    }
    public static void DestroyAllChildrenExcept(this Transform transform, Transform except, bool immediate = false)
    {
        foreach (Transform t in transform)
            if(t != except)
            {
                if (immediate)
                    Object.DestroyImmediate(t.gameObject);
                else
                    Object.Destroy(t.gameObject);
            }
    }
    public static void DestroyAllChildren(this GameObject gameObject, bool immediate = false)
    {
        gameObject.transform.DestroyAllChildren(immediate);
    }

    public static RectTransform GetRectTransform(this Component c)
    {
        return c.GetComponent<RectTransform>();
    }
    public static RectTransform GetRectTransform(this GameObject gameObject)
    {
        return gameObject.GetComponent<RectTransform>();
    }

    public static T GetComponentAndAddIfNotExist<T>(this Component c) where T : Component
    {
        T component = c.GetComponent<T>();
        if (component)
            return component;
        return c.gameObject.AddComponent<T>();
    }
    public static T GetComponentAndAddIfNotExist<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component)
            return component;
        return gameObject.AddComponent<T>();
    }
}

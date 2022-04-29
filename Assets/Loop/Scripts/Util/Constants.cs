using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EDirection : int
{
    None = -1,
    Up,
    Right,
    Down,
    Left,
    Count
}

public static class EDirectionExtension
{

    public static EDirection Next(this EDirection i) => i switch
    {
        EDirection.Up => EDirection.Right,
        EDirection.Right => EDirection.Down,
        EDirection.Down => EDirection.Left,
        EDirection.Left => EDirection.Up,
        _ => EDirection.None
    };

    public static EDirection Inverse(this EDirection i) => i switch
    {
        EDirection.Up => EDirection.Down,
        EDirection.Right => EDirection.Left,
        EDirection.Down => EDirection.Up,
        EDirection.Left => EDirection.Right,
        _ => EDirection.None
    };
}
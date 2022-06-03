using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class UserLevel
{
    [JsonIgnore] public int lastCompleted;
    [JsonIgnore] public int currentLevelId;
    public Dictionary<int,int> stars;

    public UserLevel()
    {
        stars = new Dictionary<int, int>();
    }
}
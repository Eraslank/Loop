using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using System;

public class LevelManager : MonoBehaviourSingletonPersistent<LevelManager>
{
    [SerializeField] List<TextAsset> levelJsons;

    public Dictionary<int, Level> levels = new Dictionary<int, Level>();

    public UserLevel UserLevel { get; private set; }

    public string UserSaveFile { get => Application.persistentDataPath + "/save.json"; set { } }

    public override void Awake()
    {
        base.Awake();

        if (System.IO.File.Exists(UserSaveFile))
        {
            UserLevel = JsonConvert.DeserializeObject<UserLevel>(System.IO.File.ReadAllText(UserSaveFile));
            UserLevel.lastCompleted = UserLevel.stars.Keys.Max();
        }
        else
            UserLevel = new UserLevel();
    }

    void Start()
    {
        foreach(var l in levelJsons)
        {
            var level = JsonConvert.DeserializeObject<Level>(l.text);

            levels[level.levelId] = level;
        }
    }

    public void SaveUserLevel()
    {
        System.IO.File.WriteAllText(UserSaveFile, JsonConvert.SerializeObject(UserLevel));
    }
}

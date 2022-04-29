using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviourSingletonPersistent<SceneChangeManager>
{
    public void ChangeScene(int buildIndex)
    {
        ChangeScene(SceneManager.GetSceneByBuildIndex(buildIndex).name);
    }
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

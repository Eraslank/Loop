using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private Level level;
    List<Node> levelNodes = new List<Node>();

    [SerializeField] RectTransform levelHolder;
    [SerializeField] Button nextLevelButton;
    [SerializeField] Button backButton;

    [SerializeField] Text levelText;

    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject nodePrefab;

    [Header("Post Process")]
    [SerializeField] Volume volume;
    [SerializeField] float bloomMaxIntensity;
    Sequence bloomSequence;
    Bloom bloom;
    float bloomDefValue;
    

    private void Awake()
    {
        volume.profile.TryGet(out bloom);
        if (bloom)
        {
            bloomDefValue = bloom.intensity.value;

            bloomSequence = DOTween.Sequence();
            bloomSequence.SetAutoKill(false);
            bloomSequence.SetLoops(-1, LoopType.Yoyo);
            bloomSequence.Append(DOVirtual.Float(bloomDefValue,bloomMaxIntensity,5f,(v)=> 
            {
                bloom.intensity.value = v;
            }));
            bloomSequence.AppendInterval(1f);

            bloomSequence.Pause();
        }
        nextLevelButton.gameObject.SetActive(false);
        backButton.onClick.AddListener(() =>
        {
            SceneChangeManager.Instance.ChangeScene("MainMenu");
        });
        levelNodes = Resources.FindObjectsOfTypeAll<Node>().ToList();
        GenerateRandomLevel();
    }

    public int SetNeighbors(Node node, out bool straight)
    {
        node.neighbors = new Dictionary<EDirection, Node>();
        node.neighbors[EDirection.Right] = levelNodes.FirstOrDefault(n => n.column == node.column + 1 && n.row == node.row);
        node.neighbors[EDirection.Left] = levelNodes.FirstOrDefault(n => n.column == node.column - 1 && n.row == node.row);
        node.neighbors[EDirection.Up] = levelNodes.FirstOrDefault(n => n.column == node.column && n.row == node.row - 1);
        node.neighbors[EDirection.Down] = levelNodes.FirstOrDefault(n => n.column == node.column && n.row == node.row + 1);

        straight = false;

        if (node.neighbors[EDirection.Left] && node.neighbors[EDirection.Right]
            && !node.neighbors[EDirection.Up] && !node.neighbors[EDirection.Down])
            straight = true;
        else if (node.neighbors[EDirection.Up] && node.neighbors[EDirection.Down]
            && !node.neighbors[EDirection.Left] && !node.neighbors[EDirection.Right])
            straight = true;

        return node.neighbors.Where(n => n.Value != null).Count();
    }

    public void NextLevel()
    {
        levelText.DOFade(0, .25f);
        PlayerPrefs.SetInt("LevelId", level.levelId + 1);
        nextLevelButton.gameObject.SetActive(false);
        
        bloomSequence?.Pause();
        if (bloom)
            DOVirtual.Float(bloom.intensity.value, bloomDefValue, 1f, (v) =>
               {
                   bloom.intensity.value = v;
               });
        GenerateRandomLevel();
    }

    public void CheckLevelEnd()
    {
        foreach (var node in levelNodes)
        {
            if (!node.IsDone())
            {
                return;
            }
        }
        if (PlayerPrefs.GetInt("LastCompleted") < level.levelId)
            PlayerPrefs.SetInt("LastCompleted", level.levelId);


        bloomSequence?.Restart();

        nextLevelButton.gameObject.SetActive(true);
    }

    public void GenerateLevel(Level level)
    {
        this.level = level;
        var val = levelHolder.rect.width / (float)level.column;
        ClearLevel();
        HashSet<Node> nodes = new HashSet<Node>();
        var nodeDatas = Resources.LoadAll<NodeData>("ScriptableObjects/Nodes").ToList();
        for (int i = 0; i < level.row; i++)
        {
            var row = Instantiate(rowPrefab, levelHolder).GetComponent<RectTransform>();
            row.sizeDelta = new Vector2(row.rect.width, val);
            row.localScale = Vector3.one;
            row.localPosition = Vector3.zero;
            for (int j = 0; j < level.column; j++)
            {
                if (level.levelTiles[i, j] == 0)
                {
                    var emptyRect = new GameObject("Empty").AddComponent<RectTransform>();
                    emptyRect.SetParent(row);
                    emptyRect.localScale = Vector3.one;
                    emptyRect.localPosition = Vector3.zero;
                    emptyRect.sizeDelta = Vector2.one * val;
                    continue;
                }

                var nodeRect = Instantiate(nodePrefab, row).GetComponent<RectTransform>();
                nodeRect.localScale = Vector3.one;
                nodeRect.localPosition = Vector3.zero;
                nodeRect.sizeDelta = Vector2.one * val;
                var node = nodeRect.GetComponent<Node>();
                node.row = i;
                node.column = j;
                nodes.Add(node);
            }
        }
        levelNodes = nodes.ToList();
        foreach (var n in nodes)
        {
            var neighborCount = SetNeighbors(n, out bool straight);
            n.Initialize(nodeDatas.FindAll(d => d.openTips.Count == neighborCount && d.straight == straight).RandomItem());
        }
        DOVirtual.DelayedCall(.25f, () =>
        {
            levelText.text = $"Level {level.levelId}";
            levelText.DOFade(1, .25f).SetDelay(.25f);
        });
    }

    public void ClearLevel()
    {
        levelHolder.DestroyAllChildren(true);
        if (levelHolder.childCount > 0)
            ClearLevel();
    }

    public void GenerateRandomLevel()
    {
        Level level = new Level();

        level.levelId = PlayerPrefs.GetInt("LevelId");

        Random.InitState(level.levelId);

        level.row = Random.Range(4, 7);

        do
        {
            level.column = Random.Range(4, 7);
        } while (level.column > level.row);


        level.levelTiles = new int[level.row, level.column];

        WeightedRandomBag<int> bag = new WeightedRandomBag<int>();

        bag.AddEntry(0, 2);
        bag.AddEntry(1, 11);

        for (int i = 0; i < level.row; i++)
        {
            for (int j = 0; j < level.column; j++)
            {
                level.levelTiles[i, j] = bag.GetRandom();
            }
        }


        GenerateLevel(level);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameManager gM = (GameManager)target;
        if (GUILayout.Button("Generate Level"))
            gM.GenerateRandomLevel();
        if (GUILayout.Button("Clear Level"))
            gM.ClearLevel();
    }
}

#endif
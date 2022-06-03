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

    [Header("SFX")]
    [SerializeField] List<AudioClip> levelDisappearSFX;
    [SerializeField] List<AudioClip> levelAppearSFX;
    [SerializeField] List<AudioClip> levelCompletedSFX;

    [Header("Post Process")]
    [SerializeField] Volume volume;
    [SerializeField] float bloomMaxIntensity;
    Sequence bloomSequence;
    Bloom bloom;
    float bloomDefValue;

    [SerializeField] Image star1;
    [SerializeField] Image star2;
    [SerializeField] Image star3;

    int currentStars = 3;


    private void Awake()
    {
        volume.profile.TryGet(out bloom);
        star1.fillAmount = star2.fillAmount = star3.fillAmount = 0f;
        levelText.text = "";
        levelText.color = new Color(1, 1, 1, 0);
        if (bloom)
        {
            bloomDefValue = bloom.intensity.value;

            bloomSequence = DOTween.Sequence();
            bloomSequence.SetAutoKill(false);
            bloomSequence.SetLoops(-1, LoopType.Yoyo);
            bloomSequence.Append(DOVirtual.Float(bloomDefValue, bloomMaxIntensity, 5f, (v) =>
               {
                   bloom.intensity.value = v;
               }));
            bloomSequence.AppendInterval(1f);

            bloomSequence.Pause();
        }
        nextLevelButton.gameObject.SetActive(false);
        backButton.onClick.AddListener(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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
        LevelManager.Instance.UserLevel.currentLevelId = level.levelId + 1;
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

        var userLevel = LevelManager.Instance.UserLevel;

        if (userLevel.lastCompleted < level.levelId)
            userLevel.lastCompleted = level.levelId;

        if (userLevel.stars.ContainsKey(level.levelId))
            userLevel.stars[level.levelId] = Mathf.Max(userLevel.stars[level.levelId], currentStars);
        else
            userLevel.stars.Add(level.levelId, currentStars);
                
        LevelManager.Instance.SaveUserLevel();

        SFXManager.Instance.PlaySFX(levelCompletedSFX.RandomItem());
        StopCoroutine("C_StarFill");
        starFillTween?.Pause();
        starFillTween = ((Image)starFillTween.target).DOFillAmount(1, .5f);


        bloomSequence?.Restart();

        nextLevelButton.gameObject.SetActive(true);
    }
    Tween starFillTween;
    IEnumerator C_StarFill(float startDelay)
    {
        starFillTween = star1.DOFillAmount(0, startDelay).SetEase(Ease.Linear);
        yield return starFillTween.WaitForCompletion();

        currentStars--;

        starFillTween = star2.DOFillAmount(0, 20f).SetEase(Ease.Linear);
        yield return starFillTween.WaitForCompletion();

        currentStars--;

        starFillTween = star3.DOFillAmount(0, 20f).SetEase(Ease.Linear);
        yield return starFillTween.WaitForCompletion();

        currentStars--;
    }

    public void GenerateLevel(Level level, bool inGame = true)
    {
        currentStars = 3;
        this.level = level;
        Debug.Log(levelHolder.rect.width);

        StartCoroutine(C_GenerateLevel());

        IEnumerator C_GenerateLevel()
        {
            float val = 0;
            if(inGame)
            {
                yield return StartCoroutine(C_ClearLevelAnim());
                yield return new WaitForEndOfFrame();
                val = levelHolder.rect.width;
                if (level.column == level.row)
                    val /= (float)level.column;
                else
                    val /= (float)(level.column + level.row) * .5f;
                //val = levelHolder.rect.width / (float)(/*level.row > level.column ? level.row : */level.column + level.row)*2f;
            }
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

            if (inGame)
                yield return StartCoroutine(C_GenerateLevelAnim());
            DOVirtual.DelayedCall(.25f, () =>
            {
                levelText.text = $"Level {level.levelId}";
                levelText.DOFade(1, .25f);
                Debug.Log(levelHolder.rect.width);
            });

            StartCoroutine(C_StarFill(Mathf.Clamp(nodes.Count * .9f, 8, int.MaxValue)));
        }
    }

    private IEnumerator C_ClearLevelAnim()
    {
        star1.DOFillAmount(0f, .5f).SetEase(Ease.InQuad);
        star2.DOFillAmount(0f, .5f).SetEase(Ease.InQuad);
        star3.DOFillAmount(0f, .5f).SetEase(Ease.InQuad);
        SFXManager.Instance.PlaySFX(levelDisappearSFX.RandomItem());
        yield return levelHolder.transform.DOScale(0f, .5f).SetEase(Ease.InBack).WaitForCompletion();
    }

    private IEnumerator C_GenerateLevelAnim()
    {
        star1.DOFillAmount(1, 1f).SetEase(Ease.OutQuad);
        star2.DOFillAmount(1, 1f).SetEase(Ease.OutQuad).SetDelay(.25f);
        star3.DOFillAmount(1, 1f).SetEase(Ease.OutQuad).SetDelay(.5f);
        for (int i = 0; i < levelNodes.Count; i++)
        {
            levelNodes[i].transform.localScale = Vector3.zero;
            levelNodes[i].transform.DOScale(1f, .5f).SetDelay(i*.05f).SetEase(Ease.OutBack);
            DOVirtual.DelayedCall(i * .05f, () =>
            {
                SFXManager.Instance.PlaySFX(levelAppearSFX.RandomItem());
            });
        }
        yield return new WaitForSeconds(levelNodes.Count*.05f + .75f);
    }
    public void ClearLevel()
    {
        levelHolder.DestroyAllChildren(true);
        if (levelHolder.childCount > 0)
            ClearLevel();
        levelHolder.transform.localScale = Vector3.one;
    }

    public void GenerateRandomLevel(bool inGame = true)
    {
        var levelId = LevelManager.Instance.UserLevel.currentLevelId;
        Random.InitState(levelId);

        if (LevelManager.Instance.levels.TryGetValue(levelId,out Level l))
        {
            GenerateLevel(l, inGame);
            return;
        }

        Level level = new Level();

        level.levelId = levelId;


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


        GenerateLevel(level, inGame);
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
            gM.GenerateRandomLevel(false);
        if (GUILayout.Button("Clear Level"))
            gM.ClearLevel();
    }
}

#endif
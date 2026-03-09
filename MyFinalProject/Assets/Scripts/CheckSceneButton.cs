using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckSceneButton : MonoBehaviour
{

    [Header("Final Scene Button")]
    public Button sceneButton;

    [Header("Story References")]
    public GameStory gameStory;
    public FamilyStory familyStory;

    [Header("Story Completion Flags")]
    public bool GameStoryAll = false;
    public bool FamilyStoryAll = false;

    private Coroutine showCoroutine;

    private void Start()
    {
        if (sceneButton != null)
            sceneButton.gameObject.SetActive(false);
    }

    // GameStory 完成时调用
    public void SetGameStoryDone()
    {
        GameStoryAll = true;
        RefreshCheck();
    }

    // FamilyStory 完成时调用
    public void SetFamilyStoryDone()
    {
        FamilyStoryAll = true;
        RefreshCheck();
    }

    // 检查两个是否都完成
    public void RefreshCheck()
    {
        if (!GameStoryAll || !FamilyStoryAll)
        {
            if (sceneButton != null)
                sceneButton.gameObject.SetActive(false);
            return;
        }

        if (showCoroutine != null)
            StopCoroutine(showCoroutine);

        showCoroutine = StartCoroutine(ShowButtonDelay(5f));
    }

    private IEnumerator ShowButtonDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (sceneButton != null)
            sceneButton.gameObject.SetActive(true);
    }


    //[Header("Button to Show When All Stories Completed")]
    //public Button sceneButton;  // 挂在 UI → EmptyGameObject 上的最终按钮

    //[Header("Optional Story References")]
    //public List<Story> storyList; // 所有 Story 脚本的引用，可为空，用于自动刷新

    //private HashSet<Story> completedStories = new HashSet<Story>();

    //private Coroutine showCoroutine;

    //private void Awake()
    //{
    //    if (storyList == null || storyList.Count == 0)
    //        storyList = new List<Story>(FindObjectsOfType<Story>());
    //}

    //private void Start()
    //{
    //    if (sceneButton != null)
    //        sceneButton.gameObject.SetActive(false); // 默认隐藏

    //    // 如果你在 Inspector 中手动填 storyList，可在 Start 自动刷新
    //    RefreshCheck();
    //}


    ///// Story 完成时调用

    //public void MarkStoryCompleted(Story story)
    //{
    //    if (!completedStories.Contains(story))
    //        completedStories.Add(story);

    //    RefreshCheck();
    //}


    ///// 手动或自动刷新最终按钮显示状态

    //public void RefreshCheck()
    //{
    //    if (sceneButton == null) return;

    //    // 检查 storyList 中所有 Story 是否完成
    //    if (storyList != null && storyList.Count > 0)
    //    {
    //        foreach (var s in storyList)
    //        {
    //            if (s == null) continue;
    //            if (!s.storyCompleted)
    //            {
    //                sceneButton.gameObject.SetActive(false);
    //                return;
    //            }
    //        }
    //    }

    //    //// 如果所有 story 已完成或者通过 completedStories 数量判断
    //    //sceneButton.gameObject.SetActive(true);
    //    // 所有 Story 已完成 → 延迟 5 秒显示
    //    if (showCoroutine != null) StopCoroutine(showCoroutine);
    //    showCoroutine = StartCoroutine(ShowButtonDelay(5f));
    //}



    //private IEnumerator ShowButtonDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    if (sceneButton != null)
    //        sceneButton.gameObject.SetActive(true);
    //}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [Header("Stars (请按顺序放入 star1~star5)")]
    public GameObject[] stars;

    [Header("Title 脚本引用")]
    public Title titleScript;

    private string lastTitle = "";

    void Start()
    {
        if (titleScript == null)
        {
            // 自动从子物体中查找 Title 脚本
            titleScript = GetComponentInChildren<Title>();
        }

        UpdateStars();
    }

    void Update()
    {
        if (titleScript == null) return;

        // 直接从 titleScript 的 titleText.text 读取当前显示称号
        string currentTitle = titleScript.titleText != null ? titleScript.titleText.text : "";

        if (currentTitle != lastTitle)
        {
            lastTitle = currentTitle;
            UpdateStars();
        }
    }

    void UpdateStars()
    {
        if (stars == null || stars.Length == 0) return;

        int activeCount = GetActiveStarCount(lastTitle);

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < activeCount);
        }

        Debug.Log($"[Star] 显示 {activeCount} 颗星（Title = {lastTitle}）");
    }

    int GetActiveStarCount(string title)
    {
        if (string.IsNullOrEmpty(title)) return 0;

        if (title == titleScript.level1Title) return 1;
        if (title == titleScript.level2Title) return 2;
        if (title == titleScript.level3Title) return 3;
        if (title == titleScript.level4Title) return 4;
        if (title == titleScript.level5Title) return 5;

        return 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Title : MonoBehaviour
{
    [Header("Reference")]
    public TextMeshProUGUI titleText;
    public BirdsGroup birdsGroup;
    public MusicController musicController;


    [Header("Threshold Settings")]
    public int level1Threshold = 5;
    public int level2Threshold = 15;
    public int level3Threshold = 30;
    public int level4Threshold = 50;
    public int level5Threshold = 80;

    [Header("Title Names")]
    public string level1Title = "Nobody";
    public string level2Title = "Whispered Name";
    public string level3Title = "Rising Figure";
    public string level4Title = "Local Star";
    public string level5Title = "Legend";

    private string currentTitle = "";
    public event System.Action<string> OnTitleChanged;

    /// 接收 BirdGroup 推送的总数
    public void ReceiveTotalBirds(int totalBirds)
    {
        UpdateTitleBasedOnNumber(totalBirds);
    }

    /// 根据鸟群数量判断称号等级并显示

    private void UpdateTitleBasedOnNumber(int total)
    {
        if (titleText == null) return;

        string newTitle;

        if (total < level1Threshold)
            newTitle = level1Title;
        else if (total < level2Threshold)
            newTitle = level2Title;
        else if (total < level3Threshold)
            newTitle = level3Title;
        else if (total < level4Threshold)
            newTitle = level4Title;
        else
            newTitle = level5Title;

        if (newTitle != currentTitle)
        {
            currentTitle = newTitle;
            titleText.text = currentTitle;
            // 推送给 MusicController
            OnTitleChanged?.Invoke(currentTitle);
            Debug.Log($"[Title] : {currentTitle} (TotalBirds={total})");
        }
    }

}

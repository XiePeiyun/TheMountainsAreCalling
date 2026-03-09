using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class FamilyStory : MonoBehaviour
{
    [Header("Story Buttons & Texts")]
    public List<TextMeshProUGUI> storyTexts;
    public List<Button> storyButtons;
    public List<int> levelIndex;

    public Button closeButton;
    public bool storyCompleted = false;
    public CheckSceneButton checkSceneButton;

    [Header("Main Button")]
    public Button mainButton;

    [Header("Colors")]
    public Color lockedColor = Color.white;
    public Color unlockedColor = Color.red;

    [Header("Data Input")]
    public int currentTurn = 0;  // ONLY FamilyStory 逻辑

    [Header("Auto Update Ref")]
    public NextTurn nextTurn;

    private HashSet<int> unlocked = new HashSet<int>();
    private HashSet<int> clicked = new HashSet<int>();

    private void Start()
    {
        InitButtons();
        RefreshButtons();

        if (nextTurn != null)
            nextTurn.OnTurnChanged += UpdateTurn;

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonPressed);
    }

    private void OnDestroy()
    {
        if (nextTurn != null)
            nextTurn.OnTurnChanged -= UpdateTurn;
    }

    private void UpdateTurn(int turn)
    {
        currentTurn = turn;
        RefreshButtons();
    }

    private void InitButtons()
    {
        for (int i = 0; i < storyButtons.Count; i++)
        {
            int index = i;
            storyButtons[i].onClick.AddListener(() => OnStoryClicked(index));
            SetBtn(storyButtons[i], false, lockedColor);
        }

        SetBtn(mainButton, true, lockedColor);
    }

    private void OnStoryClicked(int index)
    {
        if (!unlocked.Contains(index)) return;

        clicked.Add(index);
        ShowStory(index + 1);
        SetBtn(storyButtons[index], true, lockedColor);

        RefreshMainButton();
        CheckAllClicked();
    }

    private void RefreshButtons()
    {
        bool anyRed = false;

        for (int i = 0; i < storyButtons.Count; i++)
        {
            if (currentTurn >= levelIndex[i])
            {
                unlocked.Add(i);

                if (!clicked.Contains(i))
                {
                    SetBtn(storyButtons[i], true, unlockedColor);
                    anyRed = true;
                }
            }
        }

        SetBtn(mainButton, true, anyRed ? unlockedColor : lockedColor);
    }

    private void RefreshMainButton()
    {
        bool anyRed = false;

        foreach (int idx in unlocked)
            if (!clicked.Contains(idx))
                anyRed = true;

        SetBtn(mainButton, true, anyRed ? unlockedColor : lockedColor);
    }

    private void ShowStory(int index)
    {
        foreach (var t in storyTexts) t.gameObject.SetActive(false);
        if (index >= 1 && index <= storyTexts.Count)
            storyTexts[index - 1].gameObject.SetActive(true);
    }

    private void SetBtn(Button btn, bool interactable, Color color)
    {
        var colors = btn.colors;
        btn.interactable = interactable;

        colors.normalColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        colors.highlightedColor = interactable ? Color.yellow : Color.gray;

        btn.colors = colors;
    }

    private void CheckAllClicked()
    {
        // 如果已经完成了则不再重复执行
        if (storyCompleted) return;

        // count 被点击且有效的按钮数量（即变为白色）
        int clickedCount = 0;

        for (int i = 0; i < storyButtons.Count; i++)
        {
            // 保证索引 i 合法并且按钮存在
            if (storyButtons[i] == null) continue;

            // 只有当该按钮已经被点击（变白）时才计数
            if (clicked.Contains(i))
                clickedCount++;
        }

        // 只有当 被点击的按钮数量 等于 总按钮数量 时，才算完成
        if (clickedCount == storyButtons.Count)
        {
            storyCompleted = true;
            checkSceneButton?.SetFamilyStoryDone();

        }
    }

    public void OnCloseButtonPressed()
    {
        // 玩家点关闭时，强制进行一次检测（和原来的每次点击按钮后检测相同）
        CheckAllClicked();
        // 如果你希望关闭页面（隐藏对话框/story panel）在未完成时也能关闭，
        // 可以在这里做额外的 UI 隐藏逻辑；否则只做检测即可。
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Story : MonoBehaviour
{

    [Header("Story Buttons & Texts")]
    public List<TextMeshProUGUI> storyTexts;
    public List<Button> storyButtons;
    public List<int> levelIndex;          // 对应每个按钮的解锁数值
    public bool storyCompleted = false;
    public CheckSceneButton checkSceneButton;

    [Header("Main Buttons")]
    public Button birdMainButton;         // GameStory 主按钮（看 birdsTotal）
    public Button familyMainButton;       // FamilyStory 主按钮（看 currentTurn）

    [Header("Colors")]
    public Color lockedColor = Color.white;
    public Color unlockedColor = Color.red;

    [Header("Data Input")]
    public int birdsTotal = 0;            // GameStory 用
    public int currentTurn = 0;           // FamilyStory 用

    [Header("References for Auto-Update")]
    public BirdsGroup birdGroup;   // 指向 BirdsGroup
    public NextTurn nextTurn;      // 指向 NextTurn

    private HashSet<int> unlockedButtons = new HashSet<int>();
    private HashSet<int> clickedButtons = new HashSet<int>();

    private void Start()
    {
        InitializeButtons();
        RefreshAllButtons();

        // 自动绑定 BirdGroup / NextTurn 更新
        if (birdGroup != null)
            birdGroup.OnTotalBirdsChanged += OnBirdsTotalChanged;

        if (nextTurn != null)
            nextTurn.OnTurnChanged += OnCurrentTurnChanged;
    }

    private void OnDestroy()
    {
        if (birdGroup != null)
            birdGroup.OnTotalBirdsChanged -= OnBirdsTotalChanged;

        if (nextTurn != null)
            nextTurn.OnTurnChanged -= OnCurrentTurnChanged;
    }

    private void OnBirdsTotalChanged(int totalBirds)
    {
        birdsTotal = totalBirds;
        RefreshAllButtons();
    }

    private void OnCurrentTurnChanged(int turn)
    {
        currentTurn = turn;
        RefreshAllButtons();
    }

    /// 初始化按钮状态，全部灰色

    private void InitializeButtons()
    {
        for (int i = 0; i < storyButtons.Count; i++)
        {
            int index = i;
            if (storyButtons[i] != null)
            {
                storyButtons[i].onClick.AddListener(() => OnStoryButtonClicked(index));
                SetButtonState(storyButtons[i], false, lockedColor);
            }
        }

        if (birdMainButton != null)
            SetButtonState(birdMainButton, true, lockedColor);

        if (familyMainButton != null)
            SetButtonState(familyMainButton, true, lockedColor);
    }


    /// 点击 Story 按钮

    private void OnStoryButtonClicked(int index)
    {
        if (!unlockedButtons.Contains(index)) return;

        clickedButtons.Add(index);
        ShowStory(index + 1);
        SetButtonState(storyButtons[index], true, lockedColor);
        RefreshMainButtons();
        CheckIfAllButtonsClicked();
    }


    //更新所有按钮状态
    public void RefreshAllButtons()
    {
        UpdateButtonStates(birdsTotal, birdMainButton);     // GameStory
        UpdateButtonStates(currentTurn, familyMainButton);  // FamilyStory
    }


    /// 根据数值更新按钮解锁状态

    private void UpdateButtonStates(int value, Button mainBtn)
    {
        bool anyRed = false;

        for (int i = 0; i < storyButtons.Count && i < levelIndex.Count; i++)
        {
            var btn = storyButtons[i];
            if (btn == null) continue;

            if (value >= levelIndex[i])
            {
                if (!unlockedButtons.Contains(i))
                    unlockedButtons.Add(i);

                if (!clickedButtons.Contains(i))
                {
                    SetButtonState(btn, true, unlockedColor);
                    anyRed = true;
                }
            }
        }

        if (mainBtn != null)
            SetButtonState(mainBtn, true, anyRed ? unlockedColor : lockedColor);
    }

    private void RefreshMainButtons()
    {
        bool anyRedBird = false;
        bool anyRedFamily = false;

        for (int i = 0; i < storyButtons.Count && i < levelIndex.Count; i++)
        {
            if (unlockedButtons.Contains(i) && !clickedButtons.Contains(i))
            {
                if (birdsTotal >= levelIndex[i]) anyRedBird = true;
                if (currentTurn >= levelIndex[i]) anyRedFamily = true;
            }
        }

        if (birdMainButton != null)
            SetButtonState(birdMainButton, true, anyRedBird ? unlockedColor : lockedColor);
        if (familyMainButton != null)
            SetButtonState(familyMainButton, true, anyRedFamily ? unlockedColor : lockedColor);
    }

    private void ShowStory(int index)
    {
        foreach (var t in storyTexts)
            if (t != null) t.gameObject.SetActive(false);

        if (index >= 1 && index <= storyTexts.Count)
            storyTexts[index - 1].gameObject.SetActive(true);
    }

    private void SetButtonState(Button btn, bool interactable, Color color)
    {
        if (btn == null) return;
        btn.interactable = interactable;
        var colors = btn.colors;
        colors.normalColor = color;
        colors.pressedColor = color;
        colors.selectedColor = color;
        colors.highlightedColor = interactable ? Color.yellow : Color.gray;
        colors.colorMultiplier = 1f;
        btn.colors = colors;
    }

    private void CheckIfAllButtonsClicked()
    {
        if (storyCompleted) return;

        foreach (int idx in unlockedButtons)
            if (!clickedButtons.Contains(idx))
                return;

        storyCompleted = true;
        //checkSceneButton?.MarkStoryCompleted(this);
    }


    //[Header("Story Buttons & Texts")]
    //public List<TextMeshProUGUI> storyTexts;
    //public List<UnityEngine.UI.Button> storyButtons;
    //public List<int> levelIndex;
    //public bool storyCompleted = false;
    //public CheckSceneButton checkSceneButton;

    //[Header("Reference")]
    //public BirdsGroup birdGroup;
    //public NextTurn nextTurn;
    //public BirdKing birdKing;
    //public Button birdMainButton;              // GameStory 主按钮
    //public Button familyMainButton;            // RealStory 主按钮


    //public Color lockedColor = Color.white;
    //public Color unlockedColor = Color.red;

    //private HashSet<int> unlockedButtons = new HashSet<int>();
    //private HashSet<int> clickedButtons = new HashSet<int>();

    //[Header("Optional Settings")]
    //public bool hideAllOnStart = true;

    //private void Awake()
    //{
    //    // 清空数据，保证每次场景开始状态干净
    //    unlockedButtons.Clear();
    //    clickedButtons.Clear();
    //    storyCompleted = false;
    //}


    //private void Start()
    //{
    //    if (hideAllOnStart) HideAllTexts();
    //    InitializeButtons();

    //    // 订阅回调
    //    if (birdGroup != null)
    //        birdGroup.OnTotalBirdsChanged += HandleBirdsTotalChanged;

    //    if (nextTurn != null)
    //        nextTurn.OnTurnChanged += HandleTurnChanged;

    //    // 初始化时根据当前数据刷新按钮状态
    //    if (birdGroup != null)
    //        HandleBirdsTotalChanged(birdGroup.totalBirds);

    //    if (nextTurn != null)
    //        HandleTurnChanged(nextTurn.currentTurn);
    //}

    //// 重置 Story 状态方法（供场景切换调用）
    //public void ResetStoryState()
    //{
    //    unlockedButtons.Clear();
    //    clickedButtons.Clear();
    //    storyCompleted = false;

    //    if (hideAllOnStart)
    //        HideAllTexts();

    //    foreach (var btn in storyButtons)
    //        if (btn != null)
    //            SetButtonState(btn, false, lockedColor);

    //    if (birdMainButton != null)
    //        SetButtonState(birdMainButton, true, lockedColor);

    //    if (familyMainButton != null)
    //        SetButtonState(familyMainButton, true, lockedColor);

    //    // 根据当前数据刷新
    //    if (birdGroup != null)
    //        HandleBirdsTotalChanged(birdGroup.totalBirds);

    //    if (nextTurn != null)
    //        HandleTurnChanged(nextTurn.currentTurn);

    //    Debug.Log("Story 状态已重置");
    //}



    //private void OnDestroy()
    //{
    //    if (birdGroup != null)
    //        birdGroup.OnTotalBirdsChanged -= HandleBirdsTotalChanged;

    //    if (nextTurn != null)
    //        nextTurn.OnTurnChanged -= HandleTurnChanged;
    //}


    //// GameStory 回调
    //private void HandleBirdsTotalChanged(int totalBirds)
    //{
    //    UpdateButtonStates(totalBirds, birdMainButton);
    //}

    //// RealStory 回调
    //private void HandleTurnChanged(int currentTurn)
    //{
    //    UpdateButtonStates(currentTurn, familyMainButton);
    //    Debug.Log("HandleTurnChanged 被调用，turn = " + currentTurn);
    //}



    //private void InitializeButtons()
    //{
    //    for (int i = 0; i < storyButtons.Count; i++)
    //    {
    //        int index = i;
    //        if (storyButtons[i] != null)
    //        {
    //            storyButtons[i].onClick.AddListener(() => OnStoryButtonClicked(index));
    //            SetButtonState(storyButtons[i], false, lockedColor); // white/gray
    //        }
    //    }

    //    if (birdMainButton != null)
    //        SetButtonState(birdMainButton, true, lockedColor);

    //    if (familyMainButton != null)
    //        SetButtonState(familyMainButton, true, lockedColor);
    //}






    //private void UpdateButtonStates(int levelValue, Button mainBtn)
    //{
    //    bool anyRed = false;
    //    Debug.Log($"UpdateButtonStates: levelValue={levelValue}");

    //    for (int i = 0; i < storyButtons.Count && i < levelIndex.Count; i++)
    //    {
    //        var button = storyButtons[i];
    //        if (button == null) continue;

    //        // 达到解锁条件
    //        if (levelValue >= levelIndex[i])
    //        {
    //            if (!unlockedButtons.Contains(i))
    //            {
    //                unlockedButtons.Add(i);
    //                SetButtonState(button, true, unlockedColor); // 红色，未点击
    //            }
    //            else if (!clickedButtons.Contains(i))
    //            {
    //                SetButtonState(button, true, unlockedColor); // 解锁未点击仍为红色
    //            }
    //        }

    //        // 检查是否有红色按钮存在
    //        if (unlockedButtons.Contains(i) && !clickedButtons.Contains(i))
    //            anyRed = true;

    //        Debug.Log($"i={i}, levelIndex={levelIndex[i]}, unlocked={unlockedButtons.Contains(i)} clicked={clickedButtons.Contains(i)}");
    //    }

    //    // 更新主按钮颜色
    //    if (mainBtn != null)
    //        SetButtonState(mainBtn, true, anyRed ? unlockedColor : lockedColor);


    //}




    //private void OnStoryButtonClicked(int index)
    //{
    //    if (!unlockedButtons.Contains(index)) return;

    //    clickedButtons.Add(index); // 标记为已点击
    //    ShowStory(index + 1);

    //    // 点击后变白
    //    SetButtonState(storyButtons[index], true, lockedColor);

    //    if (birdMainButton != null)
    //        UpdateMainButtonColor(birdMainButton);

    //    if (familyMainButton != null)
    //        UpdateMainButtonColor(familyMainButton);

    //    CheckIfAllButtonsClicked();
    //}

    //private void UpdateMainButtonColor(Button mainBtn)
    //{
    //    bool anyRed = false;
    //    for (int i = 0; i < storyButtons.Count; i++)
    //    {
    //        if (unlockedButtons.Contains(i) && !clickedButtons.Contains(i))
    //        {
    //            anyRed = true;
    //            break;
    //        }
    //    }

    //    SetButtonState(mainBtn, true, anyRed ? unlockedColor : lockedColor);
    //}

    //private void ShowStory(int index)
    //{
    //    foreach (var t in storyTexts)
    //        if (t != null) t.gameObject.SetActive(false);

    //    if (index >= 1 && index <= storyTexts.Count)
    //        storyTexts[index - 1].gameObject.SetActive(true);
    //}


    //// 设置按钮颜色与交互性，不考虑悬停
    //private void SetButtonState(UnityEngine.UI.Button btn, bool interactable, Color stateColor)
    //{
    //    if (btn == null) return;

    //    btn.interactable = interactable;

    //    var colors = btn.colors;

    //    if (!interactable)
    //    {
    //        // 不可点击 → 灰色
    //        colors.normalColor = Color.gray;
    //        colors.pressedColor = Color.gray;
    //        colors.selectedColor = Color.gray;
    //        colors.highlightedColor = Color.gray; // 悬停也灰色
    //    }
    //    else
    //    {
    //        // 可点击 → 显示状态颜色（红或白）
    //        colors.normalColor = stateColor;       // 默认颜色
    //        colors.pressedColor = stateColor;      // 点击颜色不变
    //        colors.selectedColor = stateColor;     // 选中颜色
    //        colors.highlightedColor = Color.yellow; // 悬停黄色
    //    }

    //    colors.colorMultiplier = 1f;
    //    btn.colors = colors;

    //    Debug.Log($"{name} 修改了按钮：{btn.name} → interactable:{interactable}, color:{stateColor}");
    //}

    //private void HideAllTexts()
    //{
    //    foreach (var t in storyTexts)
    //        if (t != null) t.gameObject.SetActive(false);
    //}

    //private void CheckIfAllButtonsClicked()
    //{
    //    if (storyCompleted) return;

    //    foreach (int index in unlockedButtons)
    //    {
    //        if (!clickedButtons.Contains(index))
    //            return;
    //    }

    //    // 全部完成
    //    storyCompleted = true;

    //    if (checkSceneButton != null)
    //        checkSceneButton.MarkStoryCompleted(this);
    //}

}
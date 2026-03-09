using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Scout : MonoBehaviour
{
    [Header("References")]
    public Button scoutButton;
    public GameObject ratioPanel;
    public Button ratioButtonQuarter; 
    public Button ratioButtonHalf;  
    public Button ratioButtonFull; 

    private BirdsGroup birdsGroup;
    private bool isPressed = false;

    private void Start()
    {
        // 找到场景中的BirdsGroup（或通过Inspector手动拖入）
        birdsGroup = FindObjectOfType<BirdsGroup>();

        // 一开始隐藏比例按钮面板
        if (ratioPanel != null)
            ratioPanel.SetActive(false);

        // 绑定三个比例按钮事件
        ratioButtonQuarter.onClick.AddListener(() => SelectRatio(0.25f));
        ratioButtonHalf.onClick.AddListener(() => SelectRatio(0.5f));
        ratioButtonFull.onClick.AddListener(() => SelectRatio(1f));
    }

    // 当点击Scout按钮时，显示比例选项
    public void OnScoutButtonClick()
    {
        if (ratioPanel != null)
            ratioPanel.SetActive(true);
    }

    // 玩家点击比例按钮后执行
    private void SelectRatio(float ratio)
    {
        if (birdsGroup == null) return;

        int scoutCount = Mathf.RoundToInt(birdsGroup.totalBirds * ratio);
        birdsGroup.SetScoutGroup(scoutCount);

        // 隐藏按钮面板
        if (ratioPanel != null)
            ratioPanel.SetActive(false);
    }

    ///由 BirdKing.StartNewTurn() 调用，用于重置按钮状态
    /// </summary>
    public void ResetScoutButton()
    {
        isPressed = false;

        if (scoutButton != null)
            scoutButton.interactable = true;

        if (ratioPanel != null)
            ratioPanel.SetActive(false);

        Debug.Log("Scout 按钮已重置，进入新回合可再次使用。");
    }



}

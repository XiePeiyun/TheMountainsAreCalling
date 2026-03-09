using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NextTurn : MonoBehaviour
{
    [Header("Reference")]
    public BirdKing birdKing;
    public Button nextTurnButton;

    [Header("Buttons to Show/Hide")]
    public Button MoneyButton;




    [Header("DicePrfab")]
    public Dice currentWeatherDice;
    public List<Dice> huntedDiceList = new List<Dice>();
    public List<Dice> disasterDiceList = new List<Dice>();



    [Header("State")]
    public bool isPressed = false;

    public int currentTurn = 0;
    public event Action<int> OnTurnChanged;

    private void Awake()
    {
        if (birdKing != null)
            birdKing.OnStateChanged += HandleStateChanged;

        // 初始根据状态隐藏
        HandleStateChanged(birdKing.GetCurrentState());
    }

    private void HandleStateChanged(BirdKing.GameState state)
    {
        if (nextTurnButton == null) return;

        switch (state)
        {
            case BirdKing.GameState.Preparation:
                nextTurnButton.gameObject.SetActive(false);
                break;

            case BirdKing.GameState.Movement:
                nextTurnButton.gameObject.SetActive(true);
                break;

            case BirdKing.GameState.Ended:
                nextTurnButton.gameObject.SetActive(false);
                break;
        }
    }

    public void OnNextTurnClick()
    {
        if (birdKing == null)
        {
            Debug.LogWarning("NextTurn 按钮未绑定 BirdKing！");
            return;
        }

        if (birdKing.GetCurrentState() == BirdKing.GameState.Ended)
        {
            Debug.Log("当前回合已结束，请勿重复结束。");
            return;
        }

        isPressed = true;
        Debug.Log("NextTurn 按钮被点击，通知 BirdKing 结束本回合。");

        // 调用 BirdKing 的回合结束逻辑
        birdKing.EndTurn();

        currentTurn++;
        Debug.Log($"NextTurn: 当前回合 = {currentTurn}");

        OnTurnChanged?.Invoke(currentTurn);  // 通知 Story 或其他监听者

        if (currentWeatherDice != null)
        {
            currentWeatherDice.DestroyDice();
            currentWeatherDice = null;
        }

        // 销毁所有 Hunted Dice
        foreach (var dice in huntedDiceList)
        {
            if (dice != null)
                dice.DestroyDice();
        }
        huntedDiceList.Clear();

        // 销毁所有 Disaster Dice
        foreach (var dice in disasterDiceList)
        {
            if (dice != null)
                dice.DestroyDice();
        }
        disasterDiceList.Clear();


    }

    /// 禁用按钮（在回合结束后）
    public void DisableNextTurnButton()
    {
        if (nextTurnButton != null)
            nextTurnButton.interactable = false;
    }

    public void ResetNextTurnButton()
    {
        // 重置按钮信息
        isPressed = false;

        if (nextTurnButton != null)
        {
            nextTurnButton.interactable = true;
        }
        else
        {
            Debug.LogWarning("NextTurnButton 未绑定！");
        }

        // 如果 currentTurn >= 1，可以通过 Inspector 配置显示按钮
        // MoneyButton、FamilyMainButton、BirdMainButton 应该在 Inspector 的 OnClick 中设置 SetActive(true)

        Debug.Log($"NextTurn 按钮已重置，currentTurn = {currentTurn}");
    }


}

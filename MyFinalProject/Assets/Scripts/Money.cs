using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    public BirdKing birdKing;       // 手动拖入 BirdKing
    private Button btnMoney;

    [Header("Money Settings")]
    public int money = 300; 
    public int clickCost = 30; 
    public int rewardPerTurn = 10;
    public TextMeshProUGUI moneyText;


    private bool hasClickedThisTurn = false;

    private void Awake()
    {
        btnMoney = GetComponent<Button>();
        btnMoney.onClick.AddListener(OnMoneyClicked);
        UpdateMoneyText();
    }

    private void Update()
    {
        if (birdKing == null) return;


        //btnMoney.interactable = !hasClickedThisTurn;
        btnMoney.interactable = (money >= clickCost) && !hasClickedThisTurn;
    }

    /// 玩家点击 Money 按钮触发
    private void OnMoneyClicked()
    {
        if (hasClickedThisTurn) return;

        // 如果钱不够，不允许点击
        if (money < clickCost)
        {
            Debug.Log("Not enough money!");
            return;
        }

        hasClickedThisTurn = true;
        Debug.Log("Money Button Clicked!（本回合只能点一次）");

        money -= clickCost;
        UpdateMoneyText();


    }



    public void ResetMoneyButton()
    {
        hasClickedThisTurn = false;
        btnMoney.interactable = true;

        Debug.Log("Money Button Reset for New Turn");
    }

    public void AddTurnReward()
    {
        money += rewardPerTurn;
        UpdateMoneyText();
        Debug.Log($"回合奖励 +{rewardPerTurn}，当前金额: {money}");
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
            moneyText.text = $"{money} Coins";
    }
}

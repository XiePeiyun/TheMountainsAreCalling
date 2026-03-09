using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BirdsGroup : MonoBehaviour
{


    [Header("Reference")]
    public TextMeshProUGUI formulaText;
    public TextMeshProUGUI diceResultText;
    public Title title;

    [Header("FN Display")]
    public FNValue fnValue;


    [Header("Bird Data")]
    public int totalBirds = 10; 
    public int mainGroup; 
    public int scoutGroup; //a

    [Header("Bird Number UI")]
    public TextMeshProUGUI totalText;   // 显示总数
    public TextMeshProUGUI mainText;    // 显示主群数量
    public TextMeshProUGUI scoutText;   // 显示侦察数量

    [Header("Weather")]
    public int diceSteps = 0;
    public string currentWeather = "";
    public TextMeshProUGUI resultText;

    public event Action<int> OnTotalBirdsChanged;

    private void Start()
    {
        // 初始化
        mainGroup = totalBirds;
        scoutGroup = 0;
        UpdateFormula();
        UpdateBirdTexts();
    }

    public void ResetWeatherResult()
    {
        diceSteps = 0;
        currentWeather = "";

        if (resultText != null)
            resultText.text = "Pray For God";
    }

    // preparation set ScoutGroup a
    public void SetScoutGroup(int scoutCount)
    {
        scoutGroup = Mathf.Max(0, scoutCount);
        mainGroup = totalBirds - scoutGroup;
        UpdateFormula();
        UpdateBirdTexts();
        NotifyTotalBirdsChanged();
        Debug.Log($"Prepare： ScoutGroup={scoutGroup}, MainGroup={mainGroup}, Total={totalBirds}");
    }


    // Movement：Hunted b + Disaster c → new scoutGroup
    // 移动阶段更新 scoutGroup，并显示 Hunted / Disaster 结果
    public void UpdateScoutAfterMove(Hunted.HuntedOption huntedOption, Disaster.DisasterOption disasterOption)
    {
        int oldScout = scoutGroup;
        int b = huntedOption != null ? huntedOption.Value : 0;
        float c = disasterOption != null ? disasterOption.Value : 0f;

        // 计算新的 scoutGroup
        int newScout = Mathf.RoundToInt((oldScout + b) * (1 + c));
        scoutGroup = Mathf.Max(0, newScout);

        totalBirds = mainGroup + scoutGroup;

        UpdateFormulaDetailed(oldScout, b, c);

        UpdateBirdTexts();
        NotifyTotalBirdsChanged();

        // 更新掷骰结果文本
        if (diceResultText != null)
        {

            string huntedText = huntedOption != null ? $"{huntedOption.Name} (Value={huntedOption.Value:+0;-0})" : "None";

            string disasterText = disasterOption != null ? $"{disasterOption.Name} (Value={disasterOption.Value * 100:+#0.0;-#0.0}%)" : "None";

            diceResultText.text = $"Hunted: {huntedText}\nDisaster: {disasterText}";
        }


    }

    public void UpdateScoutAfterMoveForest()
    {
        int oldScout = scoutGroup;

        // Forest 安全格 → +5
        scoutGroup += 5;
        totalBirds = mainGroup + scoutGroup;

        // UI刷新
        UpdateBirdTexts();

        // 更新公式显示
        if (formulaText != null)
        {
            formulaText.text = $"ScoutGroup = {oldScout} + 5 = {scoutGroup}\nTotalBirds = {mainGroup} + {scoutGroup} = {totalBirds}";
        }

        // 通知订阅者
        NotifyTotalBirdsChanged();

        Debug.Log($"Landed on Forest: ScoutGroup {oldScout} → {scoutGroup}");
    }

    private void UpdateFormulaDetailed(int oldScout, int b, float c)
    {
        if (formulaText == null) return;

        formulaText.text =
            $"ScoutGroup = ({oldScout} + {b}) × (1 + {c * 100:+#0;-#0}%) = {scoutGroup}\n" +
            $"TotalBirds = {mainGroup} + {scoutGroup} = {totalBirds}";

        title?.ReceiveTotalBirds(totalBirds);



        if (fnValue != null)
        {
            Debug.Log($"[BirdsGroup] 调用 UpdateFNValue 一次: oldScout={oldScout}, b={b}, c={c}");
            fnValue.UpdateFNValue(oldScout, b, c);
        }


        Debug.Log($"移动后更新 ScoutGroup: {oldScout} → {scoutGroup}, TotalBirds={totalBirds}");


    }

    // 更新三个独立文本框
    public void UpdateBirdTexts()
    {
        if (totalText != null)
            totalText.text = totalBirds.ToString();

        if (mainText != null)
            mainText.text = mainGroup.ToString();

        if (scoutText != null)
            scoutText.text = scoutGroup.ToString();
    }


    public void SetWeatherSteps(int steps, string weather)
    {
        diceSteps = steps;
        currentWeather = weather;
        Debug.Log($"BirdsGroupResult: {weather} ({steps} 步)");

        if (resultText != null)
            resultText.text = $"<color=red>{currentWeather}</color> (<color=red>{diceSteps}</color>  Steps) ";
    }


    // 外部可调用合并函数（比如在Calling时使用）
    public void MergeGroups()
    {
        totalBirds = mainGroup + scoutGroup;
        mainGroup = totalBirds;
        scoutGroup = 0;
        UpdateFormula();
        UpdateBirdTexts();

        NotifyTotalBirdsChanged();
        Debug.Log($"[BirdsGroup] MergeGroups 推送 totalBirds = {totalBirds}");
    }

    public void UpdateFormula()
    {
        if (formulaText != null)
        {
            formulaText.text = $"TotalBirds = {totalBirds}\nMainGroup = {mainGroup}\nScoutGroup = {scoutGroup}";
        }

        title?.ReceiveTotalBirds(totalBirds);
    }

    private void NotifyTotalBirdsChanged()
    {
        //如果有人订阅，就通知他们
        OnTotalBirdsChanged?.Invoke(totalBirds);
    }


}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Disaster : MonoBehaviour
{
    private const float MIN_P = 0.002f;   // 0.2%
    private const float MAX_P = 0.99f;    // 99%

    [Header("Dice Spawn")]
    public Dice dicePrefab;
    public Transform diceSpawnPoint;
    public NextTurn nextTurnScript; // 拖入 NextTurn 脚本引用

    [Header("References")]
    public BirdsGroup birdsGroup;       // 引用 BirdsGroup 脚本
    public GameObject disasterUI;       // 背景 + 表格的Panel
    public List<TextMeshProUGUI> nameTexts;        // 第一列 6行
    public List<TextMeshProUGUI> valueTexts;       // 第二列 6行
    public List<TextMeshProUGUI> probabilityTexts; // 第三列 6行

    [Header("Dice Display")]
    public DiceImage2 diceImage2;

    [Header("Change Display")]
    public TextMeshProUGUI logText;
    public TextMeshProUGUI numberText;
    public GameObject logPanel;      // 包含 logText 的 Image
    public GameObject numberPanel;   // 包含 numberText 的 Image

    public float displayTime = 1.5f;

    private Coroutine hideCoroutine;

    private bool isOpen = false;

    // 数据结构，公开用于BirdsGroup调用
    [System.Serializable]
    public class DisasterOption
    {
        public string Name;
        public float Value;
        public float Probability;

        // 记录初始参数
        public float InitialValue;
        public float InitialProbability;

        public DisasterOption(string name, float value, float prob)
        {
            Name = name;
            Value = value;
            Probability = prob;

            InitialValue = value;
            InitialProbability = prob;
        }
    }

    private List<DisasterOption> disasterOptions = new List<DisasterOption>();

    private void Start()
    {
        disasterOptions = new List<DisasterOption>
        {
            new DisasterOption("Lack Forest", -0.1f, 0.40f),
            new DisasterOption("Lighting Pollution", -0.2f, 0.30f),
            new DisasterOption("High Voltage", -0.5f, 0.10f),
            new DisasterOption("Infect Plague", -0.9f, 0.05f),
            new DisasterOption("Nice Habitat", 0.6f, 0.10f),
            new DisasterOption("Group Healing", 1.2f, 0.05f)
        };

        if (disasterUI != null)
            disasterUI.SetActive(false);

        UpdateDisasterUI();

        // 一开始隐藏
        if (logPanel != null) logPanel.SetActive(false);
        if (numberPanel != null) numberPanel.SetActive(false);
    }

    public void ShowMessage(string message, string numbermessage)
    {
        // 更新文本
        if (logText != null)
            logText.text = message;
        if (numberText != null)
            numberText.text = numbermessage;

        // 显示面板
        if (logPanel != null) logPanel.SetActive(true);
        if (numberPanel != null) numberPanel.SetActive(true);

        // 如果已有隐藏协程在运行，先停止
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        // 启动隐藏协程
        hideCoroutine = StartCoroutine(HideAfterSeconds(displayTime));
    }

    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (logPanel != null) logPanel.SetActive(false);
        if (numberPanel != null) numberPanel.SetActive(false);

        hideCoroutine = null;
    }


    public void ToggleDisasterUI()
    {
        if (disasterUI == null) return;
        isOpen = !isOpen;
        disasterUI.SetActive(isOpen);
        DiceManager.Instance.TogglePanel(disasterUI);
    }

    private void UpdateDisasterUI()
    {
        for (int i = 0; i < disasterOptions.Count; i++)
        {
            var opt = disasterOptions[i];

            if (i < nameTexts.Count)
                nameTexts[i].text = opt.Name;

            if (i < valueTexts.Count)
            {
                valueTexts[i].text = $"{opt.Value * 100:+0;-0}%";

                // 比较当前值与初始值 → 改变颜色
                if (opt.Value > opt.InitialValue)
                    valueTexts[i].color = Color.red;   // 高于初始 → 红色
                else if (opt.Value < opt.InitialValue)
                    valueTexts[i].color = Color.green; // 低于初始 → 绿色
                else
                    valueTexts[i].color = Color.white; // 相同 → 白色
            }

            if (i < probabilityTexts.Count)
            {
                probabilityTexts[i].text = $"{opt.Probability * 100:F1}%";

                // 比较概率变化 → 改变颜色
                if (opt.Probability > opt.InitialProbability)
                    probabilityTexts[i].color = Color.red;
                else if (opt.Probability < opt.InitialProbability)
                    probabilityTexts[i].color = Color.green;
                else
                    probabilityTexts[i].color = Color.white;
            }




        }




    }

    // 掷 Disaster 骰子，返回新Scout数
    public int RollDisasterDice(int currentScouts)
    {
        if (birdsGroup == null || currentScouts <= 0) return 0;

        DisasterOption result = RollDisasterOption();



        int newScouts = Mathf.RoundToInt(currentScouts * (1 + result.Value));
        newScouts = Mathf.Max(0, newScouts);

        Debug.Log($"Disaster Dice: {result.Name} ({result.Value:+0.0;-0.0}) | {currentScouts} → {newScouts}");



        return newScouts;
    }

    // 掷骰并返回值 float c
    public float RollDisasterDiceFloat()
    {
        if (disasterOptions.Count == 0) return 0f;

        DisasterOption result = RollDisasterOption();
        return result != null ? result.Value : 0f;
    }

    // 掷骰并返回 DisasterOption
    public DisasterOption RollDisasterOption()
    {
        if (disasterOptions.Count == 0) return null;

        float roll = Random.value;
        float cumulative = 0f;
        DisasterOption result = null;

        foreach (var option in disasterOptions)
        {
            cumulative += option.Probability;
            if (roll <= cumulative)
            {
                result = option;

                // 直接把掷出的 Name 传给 DiceImage2
                if (diceImage2 != null)
                    diceImage2.ShowResultImage(result.Name);
                break;
            }
        }

        if (result == null)
            result = disasterOptions[disasterOptions.Count - 1];

        // **生成 Dice Prefab，并把结果传给 Dice**
        if (dicePrefab != null && diceSpawnPoint != null)
        {
            Dice newDice = Instantiate(dicePrefab, diceSpawnPoint.position, Quaternion.identity);

            newDice.diceType = Dice.DiceType.Disaster; // 标记类型
            newDice.SetResult(result.Name);           // 传递 Name 给 Dice
            newDice.DropDice();

            if (nextTurnScript != null)
                nextTurnScript.disasterDiceList.Add(newDice);
        }

        //string message = $"The {result.Name} here has affected the Scouts, resulting in a change in the number of birds {result.Value}";
        string message = $"Because of <color=red>{result.Name}</color> , the scout group <color=red>{result.Value * 100:+0;-0}%</color>";
        string numbermessage = $"<color=red>{result.Value * 100:+0;-0}%</color>";

        // 输出到控制台
        Debug.Log(message);
        Debug.Log(numbermessage);

        // 输出到 UI 文本框
        if (logText != null)
            logText.text = message;

        if (numberText != null)
            numberText.text = numbermessage;

        ShowMessage(message, numbermessage);

        return result;
    }


    // 修改某个骰子选项
    public void UpdateDisasterOption(string name, float newValue, float newProbability)
    {
        foreach (var option in disasterOptions)
        {
            if (option.Name == name)
            {
                option.Value = newValue;
                option.Probability = newProbability;
                break;
            }
        }
        UpdateDisasterUI();
    }


    public void ReceiveEvent(string eventName)
    {
        switch (eventName)
        {
            //value
            case "NoHabitatValueCut":
                NoHabitatValueCut();
                break;
            case "LightingPollutionValueCut":
                LightingPollutionValueCut();
                break;
            case "HighVoltageValueCut":
                HighVoltageValueCut();
                break;
            case "PlagueValueCut":
                PlagueValueCut();
                break;
            case "ForestValueAdd":
                ForestValueAdd();
                break;
            case "MedicalTherapyValueAdd":
                MedicalTherapyValueAdd();
                break;

                //probability
            case "ForestProbalityDouble":
                ForestProbalityDouble();
                break;
            case "MedicalTherapyProbalityDouble":
                MedicalTherapyProbalityDouble();
                break;
            case "NoHabitatProbalityHalf":
                NoHabitatProbalityHalf();
                break;
            case "LightPollutionProbalityHalf":
                LightPollutionProbalityHalf();
                break;
            case "HVProbalityHalf":
                HVProbalityHalf();
                break;
            case "PlaProbalityHalf":
                PlaProbalityHalf();
                break;
            case "HVForestProbalityDouble":
                HVForestProbalityDouble();
                break;
            case "PlaMedicalCareProbalityDouble":
                PlaMedicalCareProbalityDouble();
                break;





            // TODO: 其他事件继续添加
            default:
                Debug.LogWarning("未知事件: " + eventName);
                break;
        }
    }


    public void ForestProbalityDouble()
    {
        AdjustProbability(4, 2f);
    }

    public void MedicalTherapyProbalityDouble()
    {
        AdjustProbability(5, 2f);
    }


    public void NoHabitatProbalityHalf()
    {
        AdjustProbability(0, 0.5f);
    }

    public void LightPollutionProbalityHalf()
    {
        AdjustProbability(1, 0.5f);
    }

    public void HVProbalityHalf()
    {
        AdjustProbability(2, 0.5f);
    }

    public void PlaProbalityHalf()
    {
        AdjustProbability(3, 0.5f);
    }

    public void HVForestProbalityDouble()
    {
        AdjustProbability(2, 2f);
        AdjustProbability(4, 2f);

    }

    public void PlaMedicalCareProbalityDouble()
    {
        AdjustProbability(3, 2f);
        AdjustProbability(5, 2f);
    }





    // ===== 单独事件方法 =====
    public void NoHabitatValueCut()
    {

        disasterOptions[0].Value -= 0.1f;
        Debug.Log($"NoHabitatValueCut 执行! Name={disasterOptions[0].Name}, Steps={disasterOptions[0].Value}, Probability={disasterOptions[0].Probability}");


        UpdateDisasterUI();
    }

    public void LightingPollutionValueCut()
    {

        disasterOptions[1].Value -= 0.2f;
        Debug.Log($"LightingPollutionValueCut 执行! Name={disasterOptions[1].Name}, Steps={disasterOptions[1].Value}, Probability={disasterOptions[1].Probability}");


        UpdateDisasterUI();
    }

    public void HighVoltageValueCut()
    {

        disasterOptions[2].Value -= 0.3f;
        Debug.Log($"HighVoltageValueCut 执行! Name={disasterOptions[2].Name}, Steps={disasterOptions[2].Value}, Probability={disasterOptions[2].Probability}");


        UpdateDisasterUI();
    }

    public void PlagueValueCut()
    {

        disasterOptions[3].Value -= 0.4f;
        Debug.Log($"PlagueValueCut 执行! Name={disasterOptions[3].Name}, Steps={disasterOptions[3].Value}, Probability={disasterOptions[3].Probability}");


        UpdateDisasterUI();
    }

    public void ForestValueAdd()
    {

        disasterOptions[4].Value += 0.1f;
        Debug.Log($"ForestValueAdd 执行! Name={disasterOptions[4].Name}, Steps={disasterOptions[4].Value}, Probability={disasterOptions[4].Probability}");


        UpdateDisasterUI();
    }


    public void MedicalTherapyValueAdd()
    {

        disasterOptions[5].Value += 0.2f;
        Debug.Log($"MedicalTherapyValueAdd 执行! Name={disasterOptions[5].Name}, Steps={disasterOptions[5].Value}, Probability={disasterOptions[5].Probability}");


        UpdateDisasterUI();
    }













    private void AdjustProbability(int index, float multiplier)
    {
        float oldValue = disasterOptions[index].Probability;
        float newValue = oldValue * multiplier;
        float delta = newValue - oldValue;

        // 其他概率总和
        float totalOther = 0f;
        for (int i = 0; i < disasterOptions.Count; i++)
        {
            if (i != index) totalOther += disasterOptions[i].Probability;
        }

        // Step 2：等比缩放其他事件概率
        if (totalOther > 0f)
        {
            for (int i = 0; i < disasterOptions.Count; i++)
            {
                if (i == index) continue;

                float p = disasterOptions[i].Probability;
                p -= delta * (p / totalOther);  // 等比缩放
                disasterOptions[i].Probability = Mathf.Clamp(p, MIN_P, MAX_P);
            }
        }

        // Step 3：自身 Clamp
        newValue = Mathf.Clamp(newValue, MIN_P, MAX_P);
        disasterOptions[index].Probability = newValue;

        // Step 4：归一化，保持 sum = 1
        NormalizeDisasterProbabilities();


        //if (totalOther > 0f)
        //{
        //    for (int i = 0; i < disasterOptions.Count; i++)
        //    {
        //        if (i == index) continue;
        //        disasterOptions[i].Probability -= delta * (disasterOptions[i].Probability / totalOther);
        //        disasterOptions[i].Probability = Mathf.Max(disasterOptions[i].Probability, 0f);
        //    }
        //}

        //disasterOptions[index].Probability = newValue;

        //// 浮点归一化
        //float sum = 0f;
        //foreach (var w in disasterOptions) sum += w.Probability;
        //for (int i = 0; i < disasterOptions.Count; i++)
        //    disasterOptions[i].Probability /= sum;

        // 刷新 UI
        UpdateDisasterUI();
    }

    private void NormalizeDisasterProbabilities()
    {
        float sum = 0f;
        foreach (var d in disasterOptions)
            sum += d.Probability;

        if (sum <= 0f) return;

        // Clamp 一次，防止越界
        for (int i = 0; i < disasterOptions.Count; i++)
            disasterOptions[i].Probability = Mathf.Clamp(disasterOptions[i].Probability, MIN_P, MAX_P);

        // 再算一次总和（Clamping 后会改变总和）
        sum = 0f;
        foreach (var d in disasterOptions)
            sum += d.Probability;

        // 最终归一化
        for (int i = 0; i < disasterOptions.Count; i++)
            disasterOptions[i].Probability /= sum;
    }


}
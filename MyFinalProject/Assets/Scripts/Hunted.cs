using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class Hunted : MonoBehaviour
{

    private const float MIN_P = 0.002f;   // 0.2%
    private const float MAX_P = 0.99f;    // 99%

    [Header("Dice Spawn")]
    public Dice dicePrefab;
    public Transform diceSpawnPoint;
    public NextTurn nextTurnScript; // 拖入 NextTurn 脚本引用

    [Header("References")]
    public BirdsGroup birdsGroup;       // 引用 BirdsGroup 脚本
    public GameObject huntedUI;         // 背景 + 表格的Panel
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
    public class HuntedOption
    {
        public string Name;
        public int Value;
        public float Probability;
        public int InitialValue;
        public float InitialProbability;

        public HuntedOption(string name, int value, float probability)
        {
            Name = name;
            Value = value;
            Probability = probability;

            // 保存初始状态
            InitialValue = value;
            InitialProbability = probability;

        }
    }

    private List<HuntedOption> huntedOptions = new List<HuntedOption>();

    private void Start()
    {
        // 初始化Hunted骰子数据
        huntedOptions = new List<HuntedOption>
        {
            new HuntedOption("One-Star Hunter", -1, 0.10f),
            new HuntedOption("Two Star Hunter", -2, 0.35f),
            new HuntedOption("Three Star hunter", -3, 0.05f),
            new HuntedOption("Personal Assistance", +1, 0.10f),
            new HuntedOption("Group Rescue", +3, 0.35f),
            new HuntedOption("Social Assistance", +6, 0.05f)
        };

        if (huntedUI != null)
            huntedUI.SetActive(false);

        UpdateHuntedUI();

        // 一开始隐藏
        if (logPanel != null) logPanel.SetActive(false);
        if (numberPanel != null) numberPanel.SetActive(false);
    }

    /// 显示信息
    public void ShowMessage(string message, string numbermessage)
    {
        // 更新文本
        if (logText != null)
            logText.text = message;
        if (numberText != null)
            numberText.text = numbermessage;

        // 强制刷新 UI
        if (logPanel != null)
        {
            logPanel.SetActive(false);
            logPanel.SetActive(true);
        }
        if (numberPanel != null)
        {
            numberPanel.SetActive(false);
            numberPanel.SetActive(true);
        }

        // 停止旧协程
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




    // 切换显示/隐藏UI
    public void ToggleHuntedUI()
    {
        if (huntedUI == null) return;
        isOpen = !isOpen;
        huntedUI.SetActive(isOpen);
        DiceManager.Instance.TogglePanel(huntedUI);
    }

    // 将 huntedOptions 写入 UI 表格
    private void UpdateHuntedUI()
    {
        int count = Mathf.Min(huntedOptions.Count, nameTexts.Count, valueTexts.Count, probabilityTexts.Count);

        for (int i = 0; i < count; i++)
        {

            var option = huntedOptions[i];

            nameTexts[i].text = option.Name;

            valueTexts[i].text = option.Value.ToString();
            probabilityTexts[i].text = $"{option.Probability * 100:F1}%";

            //根据初始值变色
            if (option.Value > option.InitialValue)
                valueTexts[i].color = Color.red;
            else if (option.Value < option.InitialValue)
                valueTexts[i].color = Color.green;
            else
                valueTexts[i].color = Color.white;

            if (option.Probability > option.InitialProbability)
                probabilityTexts[i].color = Color.red;
            else if (option.Probability < option.InitialProbability)
                probabilityTexts[i].color = Color.green;
            else
                probabilityTexts[i].color = Color.white;
        }
    }

    // 掷 Hunted 骰子，返回值 b
    public int RollHuntedDice()
    {
        if (birdsGroup == null || birdsGroup.scoutGroup <= 0) return 0;

        float roll = Random.value;
        float cumulative = 0f;
        HuntedOption result = null;

        foreach (var option in huntedOptions)
        {
            cumulative += option.Probability;
            if (roll <= cumulative)
            {
                result = option;
                break;
            }
        }

        if (result == null)
            result = huntedOptions[huntedOptions.Count - 1];

        Debug.Log($"Hunted Dice: {result.Name} Value={result.Value}");
        return result.Value;



    }

    // 掷骰并返回 HuntedOption
    public HuntedOption RollHuntedOption()
    {
        if (birdsGroup == null || birdsGroup.scoutGroup <= 0) return null;

        float roll = Random.value;
        float cumulative = 0f;
        HuntedOption result = null;

        foreach (var option in huntedOptions)
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
            result = huntedOptions[huntedOptions.Count - 1];


        // **生成 Dice Prefab，并把结果传给 Dice**
        if (dicePrefab != null && diceSpawnPoint != null)
        {
            Dice newDice = Instantiate(dicePrefab, diceSpawnPoint.position, Quaternion.identity);

            newDice.diceType = Dice.DiceType.Hunted; // 标记类型
            newDice.SetResult(result.Name);           // 传递 Name 给 Dice
            newDice.DropDice();

            if (nextTurnScript != null)
                nextTurnScript.huntedDiceList.Add(newDice);
        }


        //string message = $"The {result.Name} here has affected the Scouts, resulting in a change in the number of birds {result.Value}";
        string message = $"Because of <color=red>{result.Name}</color>, the scout group <color=red>{result.Value:+0;-0}</color>";
        string numbermessage = $"<color=red>{result.Value:+0;-0}</color>";

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

    // 修改某个骰子选项的值或概率
    public void UpdateHuntedOption(string name, int newValue, float newProbability)
    {
        foreach (var option in huntedOptions)
        {
            if (option.Name == name)
            {
                option.Value = newValue;
                option.Probability = newProbability;
                break;
            }
        }
        UpdateHuntedUI();
    }


    public void ReceiveEvent(string eventName)
    {
        switch (eventName)
        {
            case "AmateurValueCut":
                AmateurValueCut();
                break;
            case "PracticeValueCut":
                PracticeValueCut();
                break;
            case "ProfessionalValueCut":
                ProfessionalValueCut();
                break;
            case "PersonValueAdd":
                PersonValueAdd();
                break;
            case "GroupValueAdd":
                GroupValueAdd();
                break;
            case "InstitutionValueAdd":
                InstitutionValueAdd();
                break;




            case "AmeInsProbalityDouble":
                AmeInsProbalityDouble();
                break;
            case "AmeInsProbalityHalf":
                AmeInsProbalityHalf();
                break;
            case "PraGroProbalityHalf":
                PraGroProbalityHalf();
                break;
            case "PraGroProbalityDouble":
                PraGroProbalityDouble();
                break;
            case "ProPerProbalityDouble":
                ProPerProbalityDouble();
                break;
            case "ProPerProbalityHalf":
                ProPerProbalityHalf();
                break;






            // TODO: 其他事件继续添加
            default:
                Debug.LogWarning("未知事件: " + eventName);
                break;
        }
    }


    public void AmeInsProbalityDouble()
    {
        AdjustProbability(0, 2f); 
        AdjustProbability(5, 2f);
    }

    public void AmeInsProbalityHalf()
    {
        AdjustProbability(0,0.5f);
        AdjustProbability(5, 0.5f);
    }

    public void PraGroProbalityHalf()
    {
        AdjustProbability(1, 0.5f);
        AdjustProbability(4, 0.5f);
    }

    public void PraGroProbalityDouble()
    {
        AdjustProbability(1, 2f);
        AdjustProbability(4, 2f);
    }

    public void ProPerProbalityDouble()
    {
        AdjustProbability(2, 2f);
        AdjustProbability(3, 2f);
    }

    public void ProPerProbalityHalf()
    {
        AdjustProbability(2, 0.5f);
        AdjustProbability(3, 0.5f);
    }






    // ===== 单独事件方法 =====
    public void AmateurValueCut()
    {

        huntedOptions[0].Value -= 1;
        Debug.Log($"AmateurValueCut 执行! Name={huntedOptions[0].Name}, Steps={huntedOptions[0].Value}, Probability={huntedOptions[0].Probability}");


        UpdateHuntedUI();
    }

    public void PracticeValueCut()
    {

        huntedOptions[1].Value -= 1;
        Debug.Log($"PracticeValueCut 执行! Name={huntedOptions[1].Name}, Steps={huntedOptions[1].Value}, Probability={huntedOptions[1].Probability}");


        UpdateHuntedUI();
    }

    public void ProfessionalValueCut()
    {

        huntedOptions[2].Value -= 1;
        Debug.Log($"ProfessionalValueCut 执行! Name={huntedOptions[2].Name}, Steps={huntedOptions[2].Value}, Probability={huntedOptions[2].Probability}");


        UpdateHuntedUI();
    }

    public void PersonValueAdd()
    {

        huntedOptions[3].Value += 1;
        Debug.Log($"PersonValueAdd 执行! Name={huntedOptions[3].Name}, Steps={huntedOptions[3].Value}, Probability={huntedOptions[3].Probability}");


        UpdateHuntedUI();
    }

    public void GroupValueAdd()
    {

        huntedOptions[4].Value += 2;
        Debug.Log($"GroupValueAdd 执行! Name={huntedOptions[4].Name}, Steps={huntedOptions[4].Value}, Probability={huntedOptions[4].Probability}");


        UpdateHuntedUI();
    }


    public void InstitutionValueAdd()
    {

        huntedOptions[5].Value += 3;
        Debug.Log($"InstitutionValueAdd 执行! Name={huntedOptions[5].Name}, Steps={huntedOptions[5].Value}, Probability={huntedOptions[5].Probability}");


        UpdateHuntedUI();
    }








    private void AdjustProbability(int index, float multiplier)
    {
        //乘以倍率
        float oldValue = huntedOptions[index].Probability;
        float newValue = oldValue * multiplier;
        float delta = newValue - oldValue;

        // 其他概率总和
        float totalOther = 0f;
        for (int i = 0; i < huntedOptions.Count; i++)
        {
            if (i != index) totalOther += huntedOptions[i].Probability;
        }


        // Step 2：对其他项做等比缩放
        if (totalOther > 0f)
        {
            for (int i = 0; i < huntedOptions.Count; i++)
            {
                if (i == index) continue;

                float p = huntedOptions[i].Probability;
                p -= delta * (p / totalOther);
                huntedOptions[i].Probability = Mathf.Clamp(p, MIN_P, MAX_P);
            }
        }

        // Step 3：自身概率Clamp
        newValue = Mathf.Clamp(newValue, MIN_P, MAX_P);
        huntedOptions[index].Probability = newValue;

        // Step 4：归一化到 1，同时保持每项不越界
        NormalizeProbabilities();


        //if (totalOther > 0f)
        //{
        //    for (int i = 0; i < huntedOptions.Count; i++)
        //    {
        //        if (i == index) continue;
        //        huntedOptions[i].Probability -= delta * (huntedOptions[i].Probability / totalOther);
        //        huntedOptions[i].Probability = Mathf.Max(huntedOptions[i].Probability, 0f);
        //    }
        //}

        //huntedOptions[index].Probability = newValue;

        //// 浮点归一化
        //float sum = 0f;
        //foreach (var w in huntedOptions) sum += w.Probability;
        //for (int i = 0; i < huntedOptions.Count; i++)
        //    huntedOptions[i].Probability /= sum;

        // 刷新 UI
        UpdateHuntedUI();
    }


    private void NormalizeProbabilities()
    {
        float sum = 0f;
        foreach (var opt in huntedOptions)
            sum += opt.Probability;

        if (sum <= 0f) return;

        // 归一化前先确保不会超界
        for (int i = 0; i < huntedOptions.Count; i++)
            huntedOptions[i].Probability = Mathf.Clamp(huntedOptions[i].Probability, MIN_P, MAX_P);

        // 再次求和
        sum = 0f;
        foreach (var opt in huntedOptions)
            sum += opt.Probability;

        // 归一化
        for (int i = 0; i < huntedOptions.Count; i++)
            huntedOptions[i].Probability /= sum;
    }

}
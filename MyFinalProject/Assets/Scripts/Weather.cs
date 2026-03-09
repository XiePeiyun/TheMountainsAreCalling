using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;


public class Weather : MonoBehaviour
{
    [Header("References")]
    private BirdsGroup birdsGroup; 
    public GameObject weatherUI; 
    public List<TextMeshProUGUI> nameTexts;        
    public List<TextMeshProUGUI> valueTexts;      
    public List<TextMeshProUGUI> probabilityTexts; 


    private bool isOpen = false;

    // 天气结果结构
    private class WeatherOption
    {
        public string Name;
        public int Steps;
        public float Probability;

        public int InitialSteps;
        public float InitialProbability;

        public WeatherOption(string name, int steps, float probability)
        {
            Name = name;
            Steps = steps;
            Probability = probability;

            InitialSteps = steps;
            InitialProbability = probability;

        }
    }

    private List<WeatherOption> weatherOptions = new List<WeatherOption>();


    private Button weatherButton;

    void Start()
    {
        // 绑定按钮
        weatherButton = GetComponent<Button>();


        // 初始化天气选项
        weatherOptions = new List<WeatherOption>
        {
            new WeatherOption("Sunny", 6, 0.05f),
            new WeatherOption("Cloud", 5, 0.15f),
            new WeatherOption("Wind", 4, 0.45f),
            new WeatherOption("Rain", 3, 0.20f),
            new WeatherOption("Snow", 2, 0.10f),
            new WeatherOption("Lighting", 1, 0.05f)
        };

        // 默认隐藏UI
        if (weatherUI != null)
            weatherUI.SetActive(false);

        UpdateWeatherUI();

    }



    void OnWeatherResult(string weather, int steps)
    {
        Debug.Log($"Weather 收到祈祷结果：{weather}, {steps}");

    }
    // 切换表格显示

    public void ToggleWeatherUI()
    {
        if (weatherUI == null)
        {
            Debug.LogWarning("Weather UI 未设置！");
            return;
        }

        isOpen = !isOpen;
        weatherUI.SetActive(isOpen);
        DiceManager.Instance.TogglePanel(weatherUI);
    }


    // 更新天气表格
    private void UpdateWeatherUI()
    {
        int count = Mathf.Min(weatherOptions.Count, nameTexts.Count, valueTexts.Count, probabilityTexts.Count);

        for (int i = 0; i < count; i++)
        {
            
            var option = weatherOptions[i];

            // 名称
            nameTexts[i].text = option.Name;

            // 步数显示与颜色
            valueTexts[i].text = option.Steps.ToString();
            if (option.Steps > option.InitialSteps)
                valueTexts[i].color = Color.red;
            else if (option.Steps < option.InitialSteps)
                valueTexts[i].color = Color.green;
            else
                valueTexts[i].color = Color.white;

            // 概率显示与颜色
            probabilityTexts[i].text = $"{option.Probability * 100f:F2}%";
            if (option.Probability > option.InitialProbability)
                probabilityTexts[i].color = Color.red;
            else if (option.Probability < option.InitialProbability)
                probabilityTexts[i].color = Color.green;
            else
                probabilityTexts[i].color = Color.white;
        }
    }


    // 通过名字修改天气的步数与概率
    public void UpdateWeatherOption(string name, int newSteps, float newProbability)
    {
        foreach (var option in weatherOptions)
        {
            if (option.Name == name)
            {
                option.Steps = newSteps;
                option.Probability = newProbability;
                break;
            }
        }

        UpdateWeatherUI();
    }



    // 获取当前概率列表
    private List<float> GetCurrentProbabilities()
    {
        List<float> probabilities = new List<float>();
        foreach (var option in weatherOptions)
        {
            probabilities.Add(option.Probability);
        }
        return probabilities;
    }

    // 应用新的概率到天气选项
    private void ApplyProbabilities(List<float> newProbabilities)
    {
        for (int i = 0; i < weatherOptions.Count && i < newProbabilities.Count; i++)
        {
            weatherOptions[i].Probability = newProbabilities[i];
        }
        UpdateWeatherUI();
    }







    // ====================== 事件接收与逻辑 ======================
    public void ReceiveEvent(string eventName)
    {
        switch (eventName)
        {
            //value
            case "SCStepCut":
                SCStepCut();
                break;
            case "CWStepCut":
                CWStepCut();
                break;
            case "RSStepAdd":
                RSStepAdd();
                break;
            case "SLStepAdd":
                SLStepAdd();
                break;
            case "SCStepAdd":
                SCStepAdd();
                break;
            case "CWStepAdd":
                CWStepAdd();
                break;




            //probability
            case "SunLightProbabilityDouble":
                SunLightProbabilityDouble();
                break;
            case "SunLightProbabilityHalf":
                SunLightProbabilityHalf();
                break;
            case "CloudSnowProbabilityDouble":
                CloudSnowProbabilityDouble();
                break;
            case "CloudSnowProbabilityHalf":
                CloudSnowProbabilityHalf();
                break;
            case "WindProbabilityDouble":
                WindProbabilityDouble();
                break;
            case "WindProbabilityHalf":
                WindProbabilityHalf();
                break;
            case "RainProbabilityDouble":
                RainProbabilityDouble();
                break;






            // TODO: 其他事件继续添加
            default:
                Debug.LogWarning("未知事件: " + eventName);
                break;
        }
    }

    // ===== 单独事件方法 =====
    public void SCStepCut()
    {
        // 假设 Sunny 对应 weatherOptions[0]
        weatherOptions[0].Steps -= 1;
        weatherOptions[1].Steps -= 1;
        Debug.Log($"SunnyStepCut 执行! Name={weatherOptions[0].Name}, Steps={weatherOptions[0].Steps}, Probability={weatherOptions[0].Probability}");
        Debug.Log($"CloudStepCut 执行! Name={weatherOptions[1].Name}, Steps={weatherOptions[1].Steps}, Probability={weatherOptions[1].Probability}");

        UpdateWeatherUI();
    }

    public void CWStepCut()
    {
        // 假设 Rain 对应 weatherOptions[3]
        weatherOptions[1].Steps -= 1;
        weatherOptions[2].Steps -= 1;
        Debug.Log($"CloudStepCut 执行! Name={weatherOptions[1].Name}, Steps={weatherOptions[1].Steps}, Probability={weatherOptions[1].Probability}");
        Debug.Log($"RainStepCut 执行! Name={weatherOptions[2].Name}, Steps={weatherOptions[2].Steps}, Probability={weatherOptions[2].Probability}");

        UpdateWeatherUI();
    }

    public void RSStepAdd()
    {
        weatherOptions[3].Steps += 1;
        weatherOptions[4].Steps += 1;

        Debug.Log($"RainStepAdd 执行! Name={weatherOptions[3].Name}, Steps={weatherOptions[3].Steps}, Probability={weatherOptions[3].Probability}");
        Debug.Log($"SnowStepAdd 执行! Name={weatherOptions[4].Name}, Steps={weatherOptions[4].Steps}, Probability={weatherOptions[4].Probability}");

        UpdateWeatherUI();
    }


    public void SLStepAdd()
    {
        weatherOptions[4].Steps += 1;
        weatherOptions[5].Steps += 1;


        Debug.Log($"SnowStepAdd 执行! Name={weatherOptions[4].Name}, Steps={weatherOptions[4].Steps}, Probability={weatherOptions[4].Probability}");
        Debug.Log($"LightingStepAdd 执行! Name={weatherOptions[5].Name}, Steps={weatherOptions[5].Steps}, Probability={weatherOptions[3].Probability}");


        UpdateWeatherUI();
    }

    public void SCStepAdd()
    {

        weatherOptions[0].Steps += 1;
        weatherOptions[1].Steps += 1;
        Debug.Log($"SunnyStepCut 执行! Name={weatherOptions[0].Name}, Steps={weatherOptions[0].Steps}, Probability={weatherOptions[0].Probability}");
        Debug.Log($"CloudStepCut 执行! Name={weatherOptions[1].Name}, Steps={weatherOptions[1].Steps}, Probability={weatherOptions[1].Probability}");

        UpdateWeatherUI();
    }


    public void CWStepAdd()
    {

        weatherOptions[1].Steps += 1;
        weatherOptions[2].Steps += 1;
        Debug.Log($"CloudStepCut 执行! Name={weatherOptions[1].Name}, Steps={weatherOptions[1].Steps}, Probability={weatherOptions[1].Probability}");
        Debug.Log($"RainStepCut 执行! Name={weatherOptions[2].Name}, Steps={weatherOptions[2].Steps}, Probability={weatherOptions[2].Probability}");

        UpdateWeatherUI();
    }










    // ===== 单独概率事件方法 =====
    public void SunLightProbabilityDouble()
    {
        AdjustProbability(0, 2f); // Sunny 概率翻倍
        AdjustProbability(5, 2f);

    }

    public void SunLightProbabilityHalf()
    {
        AdjustProbability(0, 0.5f);
        AdjustProbability(5, 0.5f);

    }

    public void CloudSnowProbabilityDouble()
    {
        AdjustProbability(1, 2f); 
        AdjustProbability(4, 2f);

    }

    public void CloudSnowProbabilityHalf()
    {
        AdjustProbability(1, 0.5f);
        AdjustProbability(4, 0.5f);

    }

    public void WindProbabilityDouble()
    {
        AdjustProbability(2, 2f);

    }

    public void WindProbabilityHalf()
    {
        AdjustProbability(2, 0.5f);

    }


    public void RainProbabilityDouble()
    {
        AdjustProbability(3, 2f); 

    }





    private void AdjustProbability(int index, float multiplier)
    {
        float oldValue = weatherOptions[index].Probability;
        float newValue = oldValue * multiplier;
        float delta = newValue - oldValue;

        // 其他概率总和
        float totalOther = 0f;
        for (int i = 0; i < weatherOptions.Count; i++)
        {
            if (i != index) totalOther += weatherOptions[i].Probability;
        }

        if (totalOther > 0f)
        {
            for (int i = 0; i < weatherOptions.Count; i++)
            {
                if (i == index) continue;
                weatherOptions[i].Probability -= delta * (weatherOptions[i].Probability / totalOther);
                weatherOptions[i].Probability = Mathf.Max(weatherOptions[i].Probability, 0f);
            }
        }

        weatherOptions[index].Probability = newValue;

        // 浮点归一化
        float sum = 0f;
        foreach (var w in weatherOptions) sum += w.Probability;
        for (int i = 0; i < weatherOptions.Count; i++)
            weatherOptions[i].Probability /= sum;

        // 刷新 UI
        UpdateWeatherUI();
    }





}

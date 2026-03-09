using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Pray : MonoBehaviour
{
    public SkyboxController skyboxController;
    private BirdsGroup birdsGroup;
    private BirdKing birdKing;
    public NextTurn nextTurnScript; // Inspector 拖入 NextTurn 脚本


    private bool isOpen = false;

    [Header("Dice Object")]
    public Dice dicePrefab;
    public Transform diceSpawnPoint;

    [Header("Dice Result")]
    public string weatherResult = "";
    public int stepsResult = 0;

    [Header("Dice Image")]
    public DiceImage diceImage;

    [Header("UI")]
    private Button prayButton;
    private bool hasPrayed = false;


    // 天气结果结构
    private class WeatherOption
    {
        public string Name;
        public int Steps;
        public float Probability;

        public WeatherOption(string name, int steps, float probability)
        {
            Name = name;
            Steps = steps;
            Probability = probability;
        }
    }

    private List<WeatherOption> weatherOptions = new List<WeatherOption>();
    void Start()
    {

        birdsGroup = FindObjectOfType<BirdsGroup>();
        birdKing = FindObjectOfType<BirdKing>();

        // 绑定按钮点击事件
        if (prayButton == null)
            prayButton = GetComponent<Button>();
        if (prayButton != null)
            prayButton.onClick.AddListener(OnPrayButtonClick);

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

    }

    //点击祈祷按钮后执行
    public void OnPrayButtonClick()
    {
        if (hasPrayed)
        {
            Debug.Log("你已经祈祷过了，本回合不能再次祈祷。");
            return;
        }



        //// dice fly to camera
        //if (dicePrefab != null && diceSpawnPoint != null)
        //{
        //    DiceUIAnimator newDice = Instantiate(dicePrefab, diceSpawnPoint.position, Quaternion.identity);
        //    if (birdKing != null && birdKing.activeCamera != null)
        //    {
        //        newDice.targetCamera = birdKing.activeCamera;
        //    }
        //    newDice.PlayFlyToCamera();
        //}



        RollWeatherDice();

        if (dicePrefab != null && diceSpawnPoint != null)
        {
            // 实例化 Dice Prefab
            Dice newDice = Instantiate(dicePrefab, diceSpawnPoint.position, Quaternion.identity);
            newDice.SetResult(weatherResult);
            newDice.DropDice();

            // 把实例保存到 NextTurn
            if (nextTurnScript != null)
                nextTurnScript.currentWeatherDice = newDice;
        }



        hasPrayed = true;
        if (prayButton != null)
            prayButton.interactable = false; // 禁用按钮
    }



    // 掷 Weather 骰子
    public void RollWeatherDice()
    {
        float roll = Random.value;
        float cumulative = 0f;
        WeatherOption result = null;

        foreach (var option in weatherOptions)
        {
            cumulative += option.Probability;
            if (roll <= cumulative)
            {
                result = option;
                break;
            }
        }

        if (result == null)
            result = weatherOptions[weatherOptions.Count - 1];

        weatherResult = result.Name;
        stepsResult = result.Steps;

        Debug.Log($"Weather Dice Result: {weatherResult} → {stepsResult} Steps");


        // 更新 BirdsGroup
        if (birdsGroup != null)
            birdsGroup.SetWeatherSteps(stepsResult, weatherResult);

        // 通知 BirdKing 更新步数
        if (birdKing != null)
            birdKing.OnPrayCompleted();

        // 显示对应图片
        if (diceImage != null)
            diceImage.ShowResultImage(weatherResult);

        if (skyboxController != null)
        {
            skyboxController.SetWeatherSky(weatherResult);
        }


    }


    ///新回合开始时重置状态（由 BirdKing.StartNewTurn() 调用）
    public void ResetPrayButton()
    {
        hasPrayed = false;

        if (prayButton != null)
            prayButton.interactable = true;

        weatherResult = "";
        stepsResult = 0;

        Debug.Log("Pray 按钮已重置，新回合可以重新祈祷。");
    }

}

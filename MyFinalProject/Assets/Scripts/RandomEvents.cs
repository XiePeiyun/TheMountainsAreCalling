using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomEvents : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject randomEventsPanel;

    [Header("Panel Buttons")]
    public Button weatherButton;
    public Button huntedButton;
    public Button disasterButton;


    [Header("Dice Scripts")]
    public Weather weather;
    public Hunted hunted;
    public Disaster disaster;

    [Header("Weather Events")]
    public List<EventItem> weatherEvents = new List<EventItem>(); // element0~9 保留编号


    [Header("Hunted Events")]
    public List<EventItem> huntedEvents = new List<EventItem>();


    [Header("Disaster Events")]
    public List<EventItem> disasterEvents = new List<EventItem>();


    [Serializable]
    public class EventItem
    {
        public string eventName;
        public TextMeshProUGUI eventText; 
    }


    // 玩家随机出的三个事件索引
    private EventItem selectedWeather;
    private EventItem selectedHunted;
    private EventItem selectedDisaster;

    // 标记是否已随机，防止重复随机
    private bool hasRandomized = false;


    void Start()
    {
        if (weatherButton != null)
            weatherButton.onClick.AddListener(() => ConfirmEvent(selectedWeather, weather));
        if (huntedButton != null)
            huntedButton.onClick.AddListener(() => ConfirmEvent(selectedHunted, hunted));
        if (disasterButton != null)
            disasterButton.onClick.AddListener(() => ConfirmEvent(selectedDisaster, disaster));
    }

    /// <summary>
    /// 打开面板并随机事件（每次打开界面随机一次）
    /// </summary>
    public void OpenRandomEventsPanel()
    {
        if (randomEventsPanel != null)
            randomEventsPanel.SetActive(true);

        if (!hasRandomized)
        {
            RandomizeEvents();
            hasRandomized = true;
        }

        // 显示随机事件文本框
        HideAllEventTexts();
        if (selectedWeather?.eventText != null) selectedWeather.eventText.gameObject.SetActive(true);
        if (selectedHunted?.eventText != null) selectedHunted.eventText.gameObject.SetActive(true);
        if (selectedDisaster?.eventText != null) selectedDisaster.eventText.gameObject.SetActive(true);
    }


    /// 随机生成三个事件

    private void RandomizeEvents()
    {
        if (weatherEvents.Count > 0)
            selectedWeather = weatherEvents[UnityEngine.Random.Range(0, weatherEvents.Count)];

        if (huntedEvents.Count > 0)
            selectedHunted = huntedEvents[UnityEngine.Random.Range(0, huntedEvents.Count)];

        if (disasterEvents.Count > 0)
            selectedDisaster = disasterEvents[UnityEngine.Random.Range(0, disasterEvents.Count)];
    }


    /// 玩家点击面板按钮确认事件
    private void ConfirmEvent(EventItem selected, MonoBehaviour targetScript)
    {
        if (selected == null || targetScript == null) return;

        HideAllEventTexts();

        // 显示选中事件文本框
        if (selected.eventText != null)
            selected.eventText.gameObject.SetActive(true);

        // 调用对应脚本的 ReceiveEvent 方法
        var method = targetScript.GetType().GetMethod("ReceiveEvent");
        if (method != null)
            method.Invoke(targetScript, new object[] { selected.eventName });
        else
            Debug.LogWarning($"Target script {targetScript.name} 没有 ReceiveEvent 方法");

        // 关闭面板
        if (randomEventsPanel != null)
            randomEventsPanel.SetActive(false);

        hasRandomized = false;
    }


    /// 隐藏所有事件文本框

    private void HideAllEventTexts()
    {
        foreach (var e in weatherEvents)
            if (e.eventText != null) e.eventText.gameObject.SetActive(false);

        foreach (var e in huntedEvents)
            if (e.eventText != null) e.eventText.gameObject.SetActive(false);

        foreach (var e in disasterEvents)
            if (e.eventText != null) e.eventText.gameObject.SetActive(false);
    }
}
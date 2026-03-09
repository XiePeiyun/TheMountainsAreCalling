using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TotalNumberTitle : MonoBehaviour
{
    public TextMeshProUGUI showNumberText;
    public BirdsGroup birdsGroup;

    private void Start()
    {
        if (birdsGroup != null)
        {
            // 订阅事件：每次 totalBirds 改变就收到推送
            birdsGroup.OnTotalBirdsChanged += UpdateNumber;
        }

        // 初始化显示
        UpdateNumber(birdsGroup.totalBirds);
    }

    private void UpdateNumber(int newTotal)
    {
        if (showNumberText != null)
        {
            showNumberText.text = newTotal.ToString();
        }
    }

    private void OnDestroy()
    {
        // 取消订阅，避免残留事件导致报错
        if (birdsGroup != null)
            birdsGroup.OnTotalBirdsChanged -= UpdateNumber;
    }


}

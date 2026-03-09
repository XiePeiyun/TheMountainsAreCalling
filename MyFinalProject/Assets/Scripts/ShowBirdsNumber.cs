using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowBirdsNumber : MonoBehaviour
{
    [Header("Text References")]
    public TextMeshProUGUI totalText;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI scoutText;

    /// <summary>
    /// 从 BirdsGroup 接收数据并更新显示
    public void ReceiveBirdData(int total, int main, int scout)
    {
        if (totalText != null)
            totalText.text = $"{total}";

        if (mainText != null)
            mainText.text = $"{main}";

        if (scoutText != null)
            scoutText.text = $"{scout}";
    }
}

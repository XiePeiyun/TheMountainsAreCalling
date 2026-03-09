using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceImage2 : MonoBehaviour
{
    [Header("Image Settings")]
    public Image diceImage;                // 显示骰子结果的UI Image
    public List<Sprite> resultSprites;     // 6个结果对应的图片
    public List<string> resultNames;       // 6个结果对应的名称

    [Header("Linked Scripts")]
    public Disaster disasterScript;        // 灾难骰脚本（可选）
    public Hunted huntedScript;            // 捕猎骰脚本（可选）

    private void Start()
    {
        if (diceImage == null)
            diceImage = GetComponent<Image>();
    }

    /// <summary>
    /// 显示灾难骰结果图像
    /// </summary>
    public void ShowDisasterResult()
    {
        if (disasterScript == null)
        {
            Debug.LogWarning("Disaster 脚本未绑定！");
            return;
        }

        var result = disasterScript.RollDisasterOption();
        if (result != null)
            ShowResultImage(result.Name);
    }

    /// <summary>
    /// 显示捕猎骰结果图像
    /// </summary>
    public void ShowHuntedResult()
    {
        if (huntedScript == null)
        {
            Debug.LogWarning("Hunted 脚本未绑定！");
            return;
        }

        var result = huntedScript.RollHuntedOption();
        if (result != null)
            ShowResultImage(result.Name);
    }

    /// <summary>
    /// 根据结果名称显示对应图片
    /// </summary>
    public void ShowResultImage(string resultName)
    {
        if (diceImage == null || resultSprites.Count != resultNames.Count)
        {
            Debug.LogError("DiceImage 未正确配置！（Sprites 和 Names 数量不一致）");
            return;
        }

        for (int i = 0; i < resultNames.Count; i++)
        {
            if (resultNames[i] == resultName)
            {
                diceImage.sprite = resultSprites[i];
                Debug.Log($"DiceImage 显示结果：{resultName}");
                return;
            }
        }

        Debug.LogWarning($"DiceImage 未找到匹配的结果：{resultName}");
    }
}

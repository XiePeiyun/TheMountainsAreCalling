using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceImage : MonoBehaviour
{
    [Header("Dice Image")]
    public Sprite sunnyImage;
    public Sprite cloudImage;
    public Sprite windImage;
    public Sprite rainImage;
    public Sprite snowImage;
    public Sprite lightingImage;

    public Sprite defaultImage;

    private Image targetImage;

    void Awake()
    {
        targetImage = GetComponent<Image>();

        if (targetImage == null)
        {
            Debug.LogError("DiceImage：当前物体缺少 Image 组件！");
        }

        // 初始化为默认图片
        if (defaultImage != null)
            targetImage.sprite = defaultImage;
    }

    /// <summary>
    /// 根据名字显示对应图片
    /// </summary>
    /// <param name="resultName">结果名称，例如 "Sunny"</param>
    public void ShowResultImage(string resultName)
    {
        if (targetImage == null) return;

        Sprite selected = GetSpriteByName(resultName);
        if (selected != null)
        {
            targetImage.sprite = selected;
        }
        else
        {
            Debug.LogWarning($"DiceImage：未找到匹配图片 → {resultName}");
            if (defaultImage != null)
                targetImage.sprite = defaultImage;
        }
    }

    /// <summary>
    /// 根据名称匹配图片
    /// </summary>
    private Sprite GetSpriteByName(string name)
    {
        switch (name)
        {
            case "Sunny": return sunnyImage;
            case "Cloud": return cloudImage;
            case "Wind": return windImage;
            case "Rain": return rainImage;
            case "Snow": return snowImage;
            case "Lighting": return lightingImage;
            default: return null;
        }
    }

    /// <summary>
    /// 重置为默认图片（例如新回合）
    /// </summary>
    public void ResetDiceImage()
    {
        if (targetImage != null && defaultImage != null)
        {
            targetImage.sprite = defaultImage;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageClick : MonoBehaviour
{
    [Header("Image")]
    public Sprite image1;     // 默认图
    public Sprite image2;     // 替换图

    public Button button;
    public Button nextTurnButton; // refresh  image1

    private Image imageComponent;
    private bool isClicked = false;

    void Start()
    {
        // 获取挂载对象的 Image
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogWarning("ImageClick 必须挂在有 Image 的对象上！");
            return;
        }

        // 设置默认图
        ResetImage();


        if (button != null)
        {
            button.onClick.AddListener(SetImage2);
        }
        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(ResetImage);
        }
    }

    public void SetImage2()
    {
        if (isClicked) return;

        if (imageComponent != null && image2 != null)
        {
            imageComponent.sprite = image2;
            isClicked = true;
        }
    }

    public void ResetImage()
    {
        if (imageComponent != null && image1 != null)
        {
            imageComponent.sprite = image1;
            isClicked = false;
        }
    }
}

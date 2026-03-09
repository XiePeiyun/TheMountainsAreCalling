using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageControl : MonoBehaviour
{
    [Header("TotalButtonImage")]
    public Sprite image1;
    public Sprite image2;
    public Sprite image3;

    [Header("ButtonReference")]
    public Button button1;
    public Button button2;
    public Button button3;

    private Image totalButtonImage;

    void Start()
    {
        // 获取 TotalButton 自身的 Image 组件
        totalButtonImage = GetComponent<Image>();

        // 绑定按钮点击事件
        if (button1 != null) button1.onClick.AddListener(() => ChangeImage(image1));
        if (button2 != null) button2.onClick.AddListener(() => ChangeImage(image2));
        if (button3 != null) button3.onClick.AddListener(() => ChangeImage(image3));
    }

    /// <summary>
    /// 切换 TotalButton 的图片
    /// </summary>
    /// <param name="newImage">要显示的新图片</param>
    void ChangeImage(Sprite newImage)
    {
        if (totalButtonImage != null && newImage != null)
        {
            totalButtonImage.sprite = newImage;
        }
    }
}

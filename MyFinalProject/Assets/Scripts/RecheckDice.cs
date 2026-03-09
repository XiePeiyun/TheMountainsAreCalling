using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecheckDice : MonoBehaviour
{
    [Header("Close Buttons")]
    public Button closeButton1;
    public Button closeButton2;

    [Header("Object to show after both clicked")]
    public GameObject targetObject;

    private bool button1Clicked = false;
    private bool button2Clicked = false;

    private void Start()
    {
        if (closeButton1 != null)
            closeButton1.onClick.AddListener(OnCloseButton1Clicked);

        if (closeButton2 != null)
            closeButton2.onClick.AddListener(OnCloseButton2Clicked);

        // 开始不显示
        if (targetObject != null)
            targetObject.SetActive(false);
    }

    private void OnCloseButton1Clicked()
    {
        button1Clicked = true;
        CheckAllClosed();
    }

    private void OnCloseButton2Clicked()
    {
        button2Clicked = true;
        CheckAllClosed();
    }

    // 两个按钮都点击 → 显示 targetObject
    private void CheckAllClosed()
    {
        if (button1Clicked && button2Clicked)
        {
            if (targetObject != null)
                targetObject.SetActive(true);

            Debug.Log("两个按钮都点击了 → targetObject 已显示");
        }
    }
}

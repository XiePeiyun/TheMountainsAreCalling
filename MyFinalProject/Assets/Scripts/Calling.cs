using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Calling : MonoBehaviour
{
    [Header("References")]
    public RectTransform buttonRect;  // UI 按钮的RectTransform
    public BirdKing birdKing;         // 引用BirdKing脚本（用来接收 Ended 状态）
    public Button uiButton;           // 按钮本身（点击事件）

    [Header("Movement Settings")]
    public float downY = 71f;       // 下滑到的目标Y位置（相对初始）
    public float slideDuration = 0.5f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector2 originalPos;
    private bool isDown = false;
    private Coroutine moveRoutine;

    private int turnCount = 0; // 当前回合计数


    private void Start()
    {
        if (buttonRect == null)
            buttonRect = GetComponent<RectTransform>();
        if (uiButton == null)
            uiButton = GetComponent<Button>();

        originalPos = buttonRect.anchoredPosition;
        uiButton.onClick.AddListener(OnButtonClicked);
    }

    // 玩家点击按钮
    private void OnButtonClicked()
    {
        if (isDown)
            SlideUp();
    }

    // 从原位置滑下
    public void SlideDown()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SlideTo(originalPos.y + downY));
        isDown = true;
    }

    // 从下方滑回原位置
    public void SlideUp()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(SlideTo(originalPos.y));
        isDown = false;
    }

    

    private IEnumerator SlideTo(float targetY)
    {
        float startY = buttonRect.anchoredPosition.y;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / slideDuration);
            float newY = Mathf.Lerp(startY, targetY, t);

            Vector2 pos = buttonRect.anchoredPosition;
            pos.y = newY;
            buttonRect.anchoredPosition = pos;

            yield return null;
        }

        Vector2 final = buttonRect.anchoredPosition;
        final.y = targetY;
        buttonRect.anchoredPosition = final;
    }

    // 外部调用：回合结束时通知
    public void OnTurnEnded()
    {
        turnCount++;
        if (turnCount >= 1) // 从第二回合开始掉落
        {
            SlideDown();
        }
    }

}

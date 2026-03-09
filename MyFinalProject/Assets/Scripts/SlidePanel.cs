using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidePanel : MonoBehaviour
{
    [Header("References")]
    public RectTransform panel;
    public Button closeButton;
    public Image overlayImage; // 需要控制透明度的Image

    [Header("Slide Positions")]
    public float hiddenX = -400f;   // 隐藏时的X坐标
    public float visibleX = 0f;     // 显示时的X坐标

    [Header("Animation")]
    public float duration = 0.5f;   // 动画时间
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade Settings")]
    public float fadeInDuration = 1f;   // Image渐显时间
    public float fadeOutDuration = 2f;  // Image渐隐时间

    private Coroutine moveRoutine;
    private Coroutine fadeRoutine;
    private bool isVisible = false;

    private void Start()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(SlideOut);

        // 初始化为隐藏位置
        Vector2 pos = panel.anchoredPosition;
        pos.x = hiddenX;
        panel.anchoredPosition = pos;

        if (overlayImage != null)
            overlayImage.gameObject.SetActive(false); // 初始隐藏

    }

    public void SlideIn()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveX(visibleX));
        isVisible = true;

        if (overlayImage != null)
        {
            overlayImage.gameObject.SetActive(true);
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeImage(overlayImage, 0f, 1f, fadeInDuration));
        }

    }

    public void SlideOut()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveX(hiddenX));
        isVisible = false;

        if (overlayImage != null)
        {
            if (fadeRoutine != null) StopCoroutine(fadeRoutine);
            fadeRoutine = StartCoroutine(FadeOutAndDisable(overlayImage, fadeOutDuration));
        }

    }

    private IEnumerator MoveX(float targetX)
    {
        float startX = panel.anchoredPosition.x;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = curve.Evaluate(elapsed / duration);
            float newX = Mathf.Lerp(startX, targetX, t);

            Vector2 pos = panel.anchoredPosition;
            pos.x = newX;
            panel.anchoredPosition = pos;

            yield return null;
        }

        Vector2 final = panel.anchoredPosition;
        final.x = targetX;
        panel.anchoredPosition = final;
    }


    private IEnumerator FadeImage(Image img, float startAlpha, float endAlpha, float fadeDuration)
    {
        Color color = img.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            img.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        img.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private IEnumerator FadeOutAndDisable(Image img, float fadeDuration)
    {
        yield return FadeImage(img, img.color.a, 0f, fadeDuration);
        img.gameObject.SetActive(false);
    }



}

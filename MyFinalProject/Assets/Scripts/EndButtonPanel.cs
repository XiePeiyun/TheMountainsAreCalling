using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndButtonPanel : MonoBehaviour
{
    [Header("Panel References")]
    public RectTransform panelGame;
    public RectTransform panelFamily;

    [Header("Close Buttons on Panels")]
    public Button closeGameButton;
    public Button closeFamilyButton;

    [Header("Final UI to Show")]
    public GameObject finalPanel; // Image2-Text2

    [Header("Slide Settings")]
    public float duration = 0.5f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float gameStartX = 600f;
    public float gameEndX = 200f;
    public float familyStartX = -600f;
    public float familyEndX = -200f;

    private bool panelGameClosed = false;
    private bool panelFamilyClosed = false;

    private void Start()
    {
        // 挂在当前Button上
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnEndButtonClick);

        // 初始化面板位置
        if (panelGame != null)
        {
            Vector2 pos = panelGame.anchoredPosition;
            pos.x = gameStartX;
            panelGame.anchoredPosition = pos;
        }

        if (panelFamily != null)
        {
            Vector2 pos = panelFamily.anchoredPosition;
            pos.x = familyStartX;
            panelFamily.anchoredPosition = pos;
        }

        if (closeGameButton != null)
            closeGameButton.onClick.AddListener(OnCloseGamePanel);

        if (closeFamilyButton != null)
            closeFamilyButton.onClick.AddListener(OnCloseFamilyPanel);

        if (finalPanel != null)
            finalPanel.SetActive(false); // 初始隐藏
    }

    private void OnEndButtonClick()
    {
        if (panelGame != null)
            StartCoroutine(SlidePanelX(panelGame, gameEndX));

        if (panelFamily != null)
            StartCoroutine(SlidePanelX(panelFamily, familyEndX));
    }

    private IEnumerator SlidePanelX(RectTransform panel, float targetX)
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

    private void OnCloseGamePanel()
    {
        panelGameClosed = true;
        CheckBothClosed();
    }

    private void OnCloseFamilyPanel()
    {
        panelFamilyClosed = true;
        CheckBothClosed();
    }

    private void CheckBothClosed()
    {
        if (panelGameClosed && panelFamilyClosed && finalPanel != null)
        {
            finalPanel.SetActive(true);
        }
    }
}

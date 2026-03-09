using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryButton : MonoBehaviour
{
    [Header("UI References")]
    public Button mainButton;       // 原始 Button
    public GameObject scrollView;   // ScrollView

    [Header("Close Button")]
    public Button CloseButton; // ScrollView 的关闭按钮

    [Header("Game Reference")]
    public BirdKing birdKing;

    private void Start()
    {
        if (mainButton != null)
            mainButton.onClick.AddListener(OnMainButtonClicked);

        if (scrollView != null)
            scrollView.SetActive(false);

        if (CloseButton != null)
        {
            CloseButton.onClick.AddListener(OnScrollCloseClicked);
        }




    }



    public void SetButtonActive(bool show)
    {
        if (mainButton != null)
            mainButton.gameObject.SetActive(show);
    }

    // 点击原始 Button
    private void OnMainButtonClicked()
    {
        if (mainButton != null)
            mainButton.gameObject.SetActive(false);

        if (scrollView != null)
            scrollView.SetActive(true);
    }

    // 点击 ScrollView 的 Close Button
    private void OnScrollCloseClicked()
    {
        if (scrollView != null)
            scrollView.SetActive(false);

        if (mainButton != null)
            mainButton.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseGuide : MonoBehaviour
{
    [Header("References")]
    public BirdKing birdKing;  // 拖入场景中的 BirdKing
    public Camera mainCamera;  // 拖入主相机（如果没拖，会在 Awake 自动获取）
    private TMP_Text tmpText;


    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (birdKing == null)
            Debug.LogWarning("MouseGuide: BirdKing 未绑定！");
    }

    private void Update()
    {
        if (birdKing == null || tmpText == null) return;

        // 跟随鼠标
        transform.position = Input.mousePosition;

        // 只在移动阶段显示
        if (birdKing.IsInMovementPhase() && birdKing.hoveredNeededSteps > 0)
        {
            tmpText.text = $"NeedSteps: {birdKing.hoveredNeededSteps}";
            tmpText.enabled = true;
        }

        else
        {
            tmpText.enabled = false;
        }
    }

}

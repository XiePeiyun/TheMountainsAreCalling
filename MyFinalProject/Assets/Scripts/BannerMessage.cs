using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BannerMessage : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject bannerPrefab;
    public float displayDuration = 3f;
    public float fadeOutTime = 0.5f;
    public bool newestOnTop = true;

    /// 显示一条新的横幅消息
    /// </summary>
    public void ShowBanner(string message)
    {
        if (bannerPrefab == null)
        {
            Debug.LogWarning("Banner Prefab 未设置！");
            return;
        }

        // 生成新的横幅
        GameObject banner = Instantiate(bannerPrefab, transform);
        banner.transform.localScale = Vector3.one;

        // 设置插入顺序（上方或下方）
        if (newestOnTop)
            banner.transform.SetAsFirstSibling();
        else
            banner.transform.SetAsLastSibling();

        // 找到 TextMeshPro 组件并设置文本
        TextMeshProUGUI text = banner.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
            text.text = message;

        // 确保存在 CanvasGroup（用于淡出）
        CanvasGroup cg = banner.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = banner.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        // 开始自动销毁流程
        StartCoroutine(FadeAndDestroy(banner, cg));
    }

    private IEnumerator FadeAndDestroy(GameObject banner, CanvasGroup cg)
    {
        // 等待显示时长
        yield return new WaitForSeconds(displayDuration);

        float elapsed = 0f;
        while (elapsed < fadeOutTime)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutTime);
            yield return null;
        }

        Destroy(banner);
    }

    // 方便编辑器测试
    [ContextMenu("Test Show Banner")]
    private void TestBanner()
    {
        ShowBanner("Test Message: <color=red>+5</color> Scouts joined!");
    }
}

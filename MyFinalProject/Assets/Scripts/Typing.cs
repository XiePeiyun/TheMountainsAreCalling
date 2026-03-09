using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Typing : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text uiText;    // TextMeshProUGUI 组件
    public Button button;      // 打字完成后显示的按钮

    [Header("Typing Settings")]
    public float typeSpeed = 0.05f; // 每个字符显示间隔（秒）

    private string fullText;

    private void Start()
    {
        if (uiText == null) return;

        fullText = uiText.text;  // 读取 Inspector 中设置的文字
        uiText.text = "";        // 初始为空

        if (button != null)
            button.gameObject.SetActive(false); // 初始隐藏按钮

        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            uiText.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }

        if (button != null)
            button.gameObject.SetActive(true);
    }
}

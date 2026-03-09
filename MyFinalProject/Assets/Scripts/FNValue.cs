using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FNValue : MonoBehaviour
{
    public BirdsGroup birdsGroup;
    public TextMeshProUGUI FNHunted;
    public TextMeshProUGUI FNDisaster;


    private Coroutine huntedRoutine;
    private Coroutine disasterRoutine;
    private Coroutine waitRoutine;

    [Header("After FN animations")]
    public Animator image0Animator;
    public Animator image1Animator;
    public float waitAfterHD = 2.5f; 
    // 标记文字动画是否完成
    private bool H = false;
    private bool D = false;



    /// 外部调用更新 FN 显示，例如在 BirdsGroup.UpdateScoutAfterMove() 之后调用

    public void UpdateFNValue(int oldScout, int b, float c)
    {
        // 重置标记
        H = false;
        D = false;

        // 一开始隐藏图片并禁用 Animator
        ResetImage(image0Animator);
        ResetImage(image1Animator);

        // 启动文字动画
        huntedRoutine = StartCoroutine(AnimateHunted(oldScout, b));
        disasterRoutine = StartCoroutine(AnimateDisaster(c));



    }

    private void ResetImage(Animator animator)
    {
        if (animator == null) return;
        animator.gameObject.SetActive(false);
        //animator.enabled = false;
    }

    private void TryPlayImageAnimation()
    {
        if (H && D)
        {
            // 文字动画完成后，等待 N 秒再播放图片动画
            StartCoroutine(WaitThenPlayImageAnimation(waitAfterHD));
        }
    }


    private IEnumerator WaitThenPlayImageAnimation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (image0Animator != null) StartCoroutine(PlayImageAnimation(image0Animator));
        if (image1Animator != null) StartCoroutine(PlayImageAnimation(image1Animator));
    }

    private IEnumerator PlayImageAnimation(Animator animator)
    {
        if (animator == null) yield break;

        // 显示 Image，但不禁用 Animator
        animator.gameObject.SetActive(true);

        // 播放动画，从第一帧开始
        animator.Play(0, -1, 0f);

        // 等待一帧，让 Animator 进入状态
        yield return null;

        // 等待动画完整播放
        yield return new WaitUntil(() =>
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            return state.normalizedTime >= 1f && !animator.IsInTransition(0);
        });

        // 播放完成后隐藏 Image
        animator.gameObject.SetActive(false);

    }



    private IEnumerator AnimateHunted(int oldScout, int b)
    {
        if (FNHunted == null) yield break;

        int result = oldScout + b;

        // Step 1: 显示 oldScout
        FNHunted.text = $"({oldScout})";
        yield return new WaitForSeconds(0.5f);

        // Step 2: 若 b != 0，显示 + b
        if (b != 0)
        {
            FNHunted.text = $"({oldScout} + {b})";
            yield return new WaitForSeconds(0.5f);
        }

        // Step 3: 显示最终结果
        FNHunted.text = $"<color=red>{result}</color>";

        // 标记完成
        H = true;
        TryPlayImageAnimation();
    }

    private IEnumerator AnimateDisaster(float c)
    {
        if (FNDisaster == null) yield break;

        float percentC = c * 100f;
        float result = (1 + c) * 100f;

        // Step 1: 显示初始 100%
        FNDisaster.text = "(100%)";
        yield return new WaitForSeconds(0.5f);

        // Step 2: 若 c != 0，显示 + c%
        if (Mathf.Abs(c) > 0.0001f)
        {
            FNDisaster.text = $"(100% + {percentC:+#0;-#0}%)";
            yield return new WaitForSeconds(0.5f);
        }

        // Step 3: 显示最终结果
        FNDisaster.text = $"<color=red>{result:+#0;-#0}%</color>";

        // 标记完成
        D = true;
        TryPlayImageAnimation();
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomProbability : MonoBehaviour
{
    /// 按指定索引翻倍（或倍数调整）某个概率，并按比例扣减其他概率，保证总和为1
    /// </summary>
    /// <param name="probabilities">概率列表（0~1）</param>
    /// <param name="index">要调整的索引</param>
    /// <param name="multiplier">倍数，如2表示翻倍</param>
    public static void AdjustProbability(List<float> probabilities, int index, float multiplier)
    {
        if (probabilities == null || probabilities.Count == 0)
        {
            Debug.LogWarning("probability null");
            return;
        }

        if (index < 0 || index >= probabilities.Count)
        {
            Debug.LogWarning("Index out of bounds");
            return;
        }

        float oldValue = probabilities[index];
        float newValue = oldValue * multiplier;
        float delta = newValue - oldValue;

        // 其他元素总和
        float totalOther = 0f;
        for (int i = 0; i < probabilities.Count; i++)
        {
            if (i != index)
                totalOther += probabilities[i];
        }

        if (totalOther <= 0f)
        {
            Debug.LogWarning("The sum of the other elements is 0, so they cannot be deducted proportionally.");
            probabilities[index] = Mathf.Clamp01(newValue);
            return;
        }

        // 调整其他元素
        for (int i = 0; i < probabilities.Count; i++)
        {
            if (i == index) continue;
            probabilities[i] -= delta * (probabilities[i] / totalOther);
            probabilities[i] = Mathf.Max(probabilities[i], 0f); // 防止负值
        }

        probabilities[index] = newValue;

        // 修正总和浮点误差
        float sum = 0f;
        foreach (var p in probabilities) sum += p;
        for (int i = 0; i < probabilities.Count; i++)
        {
            probabilities[i] /= sum;
        }
    }

    /// <summary>
    /// 根据概率数组随机选择一个索引
    /// </summary>
    public static int Roll(List<float> probabilities)
    {
        float rand = Random.value;
        float cumulative = 0f;
        for (int i = 0; i < probabilities.Count; i++)
        {
            cumulative += probabilities[i];
            if (rand <= cumulative)
                return i;
        }
        return probabilities.Count - 1; // 万一浮点误差
    }
}

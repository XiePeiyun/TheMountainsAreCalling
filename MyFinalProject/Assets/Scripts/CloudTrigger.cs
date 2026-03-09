using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudTrigger : MonoBehaviour
{
    [Header("References")]
    public BirdKing player;             // 玩家
    public EndFailGame endFailGame;     // 指向 EndFailGame 脚本

    [Header("Settings")]
    public float delay = 3f;             // 延迟时间（秒）
    private bool hasTriggered = false;   // 防止重复触发

    private void Update()
    {
        if (player == null || endFailGame == null || hasTriggered) return;

        // 获取玩家位置
        Vector3 playerPos = player.transform.position;

        // 判断玩家是否在 Cloud 的 BoxCollider 范围内
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null && box.bounds.Contains(playerPos))
        {
            // 玩家在云层内，开始延迟触发
            StartCoroutine(TriggerFailDelayed());
        }
    }

    private IEnumerator TriggerFailDelayed()
    {
        hasTriggered = true;  // 防止多次触发

        // 等待指定时间
        yield return new WaitForSeconds(delay);

        // 调用 EndFailGame 的方法显示 gameObject2
        endFailGame.ActivateGameObject2();
        Debug.Log($"玩家进入云层 {delay} 秒后，显示 EndGame2！");
    }


    //private void Update()
    //{
    //    if (player == null || endFailGame == null) return;

    //    // 获取玩家位置
    //    Vector3 playerPos = player.transform.position;

    //    // 判断玩家是否在 Cloud 的 BoxCollider 范围内
    //    BoxCollider box = GetComponent<BoxCollider>();
    //    if (box != null && box.bounds.Contains(playerPos))
    //    {
    //        // 玩家在云层内，触发失败
    //        TriggerFail();
    //    }
    //}

    //private void TriggerFail()
    //{
    //    // 调用 EndFailGame 的方法显示 gameObject2
    //    endFailGame.ActivateGameObject2();
    //    Debug.Log("玩家在云层内，显示 EndGame2！");
    //}
}

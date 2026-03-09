using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteGroundLooper : MonoBehaviour
{
    //[Header("NextTurn Reference")]
    //public NextTurn nextTurn;
    //private int lastTurnChecked = 0;

    [Header("Buttons trigger ground change")]
    public List<Button> triggerButtons;

    private int clickCount = 0;        // 玩家点击计数
    public int clicksNeeded = 3;       // 需要几次点击才触发一次地图改变

    [Header("Ground Prefabs in order (prefab1 → prefab4)")]
    public GameObject[] groundPrefabs; // prefab1~4

    [Header("Active ground instances")]
    public List<GameObject> activeGrounds = new List<GameObject>();

    [Header("Prefab Destroy Animation")]
    public float upY = 3f;            // 上升目标Y
    public float downY = -8f;         // 下降目标Y
    public float upDuration = 0.5f;   // 上升时间
    public float downDuration = 1.5f; // 下降时间
    [Header("Prefab Destroy Rotation")]
    public float rotateDuration = 2f;  // 旋转时间（秒）
    public float rotateAngle = 180f;   // 绕 X 轴旋转角度

    [Header("Cloud Settings")]
    public Transform cloud;               // 云对象（包含 BoxCollider isTrigger）
    public float cloudOffsetZ = -5f;      // 初始 cloud 位置 = -5
    public float cloudMoveZ = 5f;         // 每次前移5



    [Header("Settings")]
    public float prefabLength = 5f;      // 每块 prefab 沿 Z 轴长度
    public int loopEveryRounds = 3;      // 每几个回合循环一次

    private int prefabIndex = 0;         // 下一个生成 prefab 的索引
    private int lastHandledRound = 0;

    private Coroutine cloudMoveCoroutine;

    private void Start()
    {
        //if (nextTurn != null)
        //    nextTurn.OnTurnChanged += HandleTurnChanged;

        // 给每个按钮加监听
        foreach (Button btn in triggerButtons)
        {
            if (btn != null)
                btn.onClick.AddListener(OnAnyButtonClicked);
        }


        // 设置 cloud 的初始位置
        if (cloud != null)
        {
            Vector3 pos = cloud.position;
            pos.z = cloudOffsetZ;
            cloud.position = pos;
        }
    }



    private void OnDestroy()
    {
        //if (nextTurn != null)
        //    nextTurn.OnTurnChanged -= HandleTurnChanged;

        // 移除监听（避免报错）
        foreach (Button btn in triggerButtons)
        {
            if (btn != null)
                btn.onClick.RemoveListener(OnAnyButtonClicked);
        }
    }

    private void OnAnyButtonClicked()
    {
        clickCount++;
        Debug.Log($"Button clicked {clickCount}/{clicksNeeded}");

        if (clickCount >= clicksNeeded)
        {
            clickCount = 0;   // 重置计数
            LoopGround();     // 执行地图改变
        }
    }

    //private void HandleTurnChanged(int currentTurn)
    //{
    //    if (currentTurn == 0) return;

    //    if (currentTurn - lastHandledRound >= loopEveryRounds)
    //    {
    //        lastHandledRound = currentTurn;
    //        LoopGround();
    //    }
    //}

    private void MoveCloudSmoothly(float moveZ)
    {
        if (cloudMoveCoroutine != null)
            StopCoroutine(cloudMoveCoroutine);

        cloudMoveCoroutine = StartCoroutine(CloudMoveCoroutine(moveZ));
    }

    private void LoopGround()
    {
        if (activeGrounds.Count == 0) return;

        GameObject firstTile = activeGrounds[0];
        activeGrounds.RemoveAt(0);
        StartCoroutine(MoveAndRotateAndRecycle(firstTile));


        if (cloud != null)
        {
            MoveCloudSmoothly(cloudMoveZ);
            Debug.Log($"Cloud moving forward {cloudMoveZ} units");
        }

    }

    private IEnumerator MoveAndRotateAndRecycle(GameObject tile)
    {
        if (tile == null) yield break;

        Vector3 startPos = tile.transform.position;
        Quaternion startRot = tile.transform.rotation;

        // ---------- 1. 上升动画 ----------
        Vector3 upPos = new Vector3(startPos.x, upY, startPos.z);
        Quaternion upRot = startRot * Quaternion.Euler(rotateAngle / 2f, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < upDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / upDuration;
            tile.transform.position = Vector3.Lerp(startPos, upPos, t);
            tile.transform.rotation = Quaternion.Slerp(startRot, upRot, t);
            yield return null;
        }

        // ---------- 2. 下降动画 ----------
        Vector3 downPos = new Vector3(startPos.x, downY, startPos.z);
        Quaternion endRot = startRot * Quaternion.Euler(rotateAngle, 0f, 0f);

        elapsed = 0f;
        while (elapsed < downDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / downDuration;
            tile.transform.position = Vector3.Lerp(upPos, downPos, t);
            tile.transform.rotation = Quaternion.Slerp(upRot, endRot, t);
            yield return null;
        }

        // ---------- 3. 动画结束后，播放生成动画，并放回队尾 ----------
        GameObject lastTile = activeGrounds[activeGrounds.Count - 1];
        Vector3 newPos = lastTile.transform.position;
        newPos.z += prefabLength;   // 队尾位置

        yield return StartCoroutine(SpawnReverseAnimation(tile, newPos));

        activeGrounds.Add(tile);

        Debug.Log($"Tile recycled to Z = {newPos.z}");

        //// ---------- 3. 动画结束后，不销毁，直接移到队尾 ----------
        //GameObject lastTile = activeGrounds[activeGrounds.Count - 1];
        //Vector3 newPos = lastTile.transform.position;
        //newPos.z += prefabLength;   // 移到队尾应该出现的位置

        //tile.transform.SetPositionAndRotation(
        //    newPos,
        //    Quaternion.identity
        //);

        //// 加回列表末尾
        //activeGrounds.Add(tile);

        //Debug.Log($"Tile recycled to Z = {newPos.z}");
    }

    private IEnumerator SpawnReverseAnimation(GameObject tile, Vector3 targetPos)
    {
        // Step 1：先把 tile 放到队尾，但 Y = downY（和销毁动画的终点一致）
        Vector3 posDown = new Vector3(targetPos.x, downY, targetPos.z);
        Vector3 posUp = new Vector3(targetPos.x, upY, targetPos.z);
        Vector3 posZero = new Vector3(targetPos.x, 0f, targetPos.z);

        tile.transform.SetPositionAndRotation(posDown, Quaternion.identity);

        // Step 2：播放下降动画的反向（downY → upY）
        float elapsed = 0f;
        while (elapsed < downDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / downDuration;

            tile.transform.position = Vector3.Lerp(posDown, posUp, t);
            yield return null;
        }

        // Step 3：播放上升动画的反向（upY → 0）
        elapsed = 0f;
        while (elapsed < upDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / upDuration;

            tile.transform.position = Vector3.Lerp(posUp, posZero, t);
            yield return null;
        }

        tile.transform.position = posZero;
    }

    private IEnumerator CloudMoveCoroutine(float moveZ)
    {
        Vector3 startPos = cloud.position;
        Vector3 endPos = startPos + new Vector3(0, 0, moveZ);

        float duration = 0.5f; // 平滑时间，0.5秒
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cloud.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            yield return null;
        }

        cloud.position = endPos;
    }
}

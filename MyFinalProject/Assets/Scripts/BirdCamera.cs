using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
//using static UnityEditorInternal.VersionControl.ListControl;

public class BirdCamera : MonoBehaviour
{
    public BirdKing birdKing;

    [Header("Cameras")]
    public Camera camera1; // maincamera（Movement）
    public Camera camera2; // modelcamera（Preparation）

    [Header("Positions")]
    public Transform camera1FollowPos; // camera1 跟随位置
    public Transform camera2HomePos;   // camera2 初始位置

    [Header("Settings")]
    public float moveDuration = 1.2f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isMoving = false;

    private void Start()
    {
        // 准备阶段：camera2 on，camera1 off
        camera2.transform.position = camera2HomePos.position;
        camera2.transform.rotation = camera2HomePos.rotation;
        SetActiveCamera(camera2);
    }

    //Preparation -> Movement
    public void OnMovementPhase()
    {
        if (!isMoving)
            StartCoroutine(MoveCameraToPosition(camera2, camera1FollowPos.position, camera1FollowPos.rotation, camera1));
    }


    // Movement -> Preparation

    public void OnPreparationPhase()
    {
        if (!isMoving)
            StartCoroutine(MoveCameraToPosition(camera1, camera2HomePos.position, camera2HomePos.rotation, camera2));
    }

    private IEnumerator MoveCameraToPosition(Camera movingCam, Vector3 targetPos, Quaternion targetRot, Camera targetCam)
    {
        isMoving = true;

        // 保存移动相机的初始位置（用于返回）
        Vector3 movingCamStartPos = movingCam.transform.position;
        Quaternion movingCamStartRot = movingCam.transform.rotation;

        // 确保移动相机激活且在最前面
        movingCam.enabled = true;
        movingCam.depth = 10;

        // 目标相机暂时关闭
        targetCam.enabled = false;
        targetCam.depth = 0;

        // 平滑移动
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float progress = moveCurve.Evaluate(t / moveDuration);

            movingCam.transform.position = Vector3.Lerp(movingCamStartPos, targetPos, progress);
            movingCam.transform.rotation = Quaternion.Slerp(movingCamStartRot, targetRot, progress);
            yield return null;
        }

        // 确保最终位置准确
        movingCam.transform.position = targetPos;
        movingCam.transform.rotation = targetRot;

        // 切换到目标相机
        SetActiveCamera(targetCam);

        // 移动相机回到原位但不显示
        movingCam.transform.position = movingCamStartPos;
        movingCam.transform.rotation = movingCamStartRot;
        movingCam.enabled = false;
        movingCam.depth = 0;

        isMoving = false;
    }

    private void SetActiveCamera(Camera activeCam)
    {
        // 设置相机激活状态
        activeCam.enabled = true;
        activeCam.depth = 10;

        // 更新birdKing的引用
        birdKing.activeCamera = activeCam;
    }




    //public BirdKing birdKing;

    //[Header("Cameras")]
    //public Camera camera1; // 主相机（Movement）
    //public Camera camera2; // 准备相机（Preparation）

    //[Header("Positions")]
    //public Transform camera1FollowPos;      // camera1 的位置（camera2 要移过去的位置）
    //public Transform camera2HomePos;        // camera2 原始位置

    //[Header("Settings")]
    //public float moveDuration = 1.2f;
    //public AnimationCurve moveCurve;

    //[Header("Blend")]
    //public float blendDuration = 1.2f;
    //public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    //private bool isBlending = false;

    //private bool isMoving = false;



    //private void Start()
    //{
    //    // 初始：展示 camera2
    //    ShowCamera2();



    //    // birdKing 使用 camera2 进行 Raycast
    //    birdKing.activeCamera = camera2;

    //    // 确保 camera2 在准备位置
    //    camera2.transform.position = camera2HomePos.position;
    //    camera2.transform.rotation = camera2HomePos.rotation;
    //}

    //// 显示/隐藏相机
    //private void ShowCamera1()
    //{
    //    camera1.enabled = true;
    //    camera1.depth = 10;

    //    camera2.enabled = false;
    //    camera2.depth = 0;
    //}

    //private void ShowCamera2()
    //{
    //    camera2.enabled = true;
    //    camera2.depth = 10;

    //    camera1.enabled = false;
    //    camera1.depth = 0;
    //}



    // //Preparation → Movement

    //public void OnMovementPhase()
    //{
    //    if (!isMoving)
    //        StartCoroutine(PreparationToMovement());
    //}

    //private IEnumerator PreparationToMovement()
    //{
    //    isMoving = true;

    //    // ① camera2 移动到 camera1 位置
    //    yield return StartCoroutine(MoveCamera(
    //        camera2.transform,
    //        camera1FollowPos.position,
    //        camera1FollowPos.rotation
    //    ));

    //    // ② 切换画面到 camera1
    //    ShowCamera1();

    //    birdKing.activeCamera = camera1;
    //    isMoving = false;
    //}



    //// Movement → NewTurn

    //public void OnPreparationPhase()
    //{
    //    if (!isMoving)
    //        StartCoroutine(MovementToPreparation());
    //}

    //private IEnumerator MovementToPreparation()
    //{
    //    isMoving = true;

    //    // 切回 camera2 显示
    //    ShowCamera2();
    //    birdKing.activeCamera = camera2;

    //    // camera2 移回它的原点位置
    //    yield return StartCoroutine(MoveCamera(
    //        camera2.transform,
    //        camera2HomePos.position,
    //        camera2HomePos.rotation
    //    ));

    //    isMoving = false;
    //}


    //// 相机移动（平滑）

    //private IEnumerator MoveCamera(Transform cam, Vector3 targetPos, Quaternion targetRot)
    //{
    //    Vector3 startPos = cam.position;
    //    Quaternion startRot = cam.rotation;

    //    float t = 0f;

    //    while (t < moveDuration)
    //    {
    //        t += Time.deltaTime;
    //        float p = moveCurve.Evaluate(t / moveDuration);

    //        cam.position = Vector3.Lerp(startPos, targetPos, p);
    //        cam.rotation = Quaternion.Lerp(startRot, targetRot, p);

    //        yield return null;
    //    }
    //}



}

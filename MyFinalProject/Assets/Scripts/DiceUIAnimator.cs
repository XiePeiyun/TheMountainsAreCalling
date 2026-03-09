using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceUIAnimator : MonoBehaviour
{
    public float flySpeed = 8f;
    public float rotateSpeed = 360f;
    public float disappearDistance = 0.5f;

    private Rigidbody rb;
    private bool isFlying = false;
    private Transform cam;
    public Camera targetCamera;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = targetCamera.transform;
    }

    public void PlayFlyToCamera()
    {
        if (isFlying) return;

        isFlying = true;

        //关键：关闭物理，否则会掉下去
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    void Update()
    {
        if (!isFlying) return;

        // 朝向摄像机飞行
        transform.position = Vector3.MoveTowards(
            transform.position,
            cam.position,
            flySpeed * Time.deltaTime
        );

        // 自旋
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        // 到达相机前方一定距离后销毁
        float dist = Vector3.Distance(transform.position, cam.position);
        if (dist < disappearDistance)
        {
            Destroy(gameObject);
        }
    }
}

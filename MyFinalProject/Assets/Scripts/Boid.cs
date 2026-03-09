using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 position;
    public Vector3 forward;
    public float speed;

    public void UpdateBoid(Vector3 acceleration, float minSpeed, float maxSpeed)
    {
        speed += acceleration.magnitude * Time.deltaTime;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        forward = (forward + acceleration * Time.deltaTime).normalized;
        position += forward * speed * Time.deltaTime;

        transform.position = position;
        transform.forward = forward;
    }

    public bool IsHeadingForCollision(float radius, float avoidDistance, LayerMask mask)
    {
        return Physics.SphereCast(position, radius, forward, out _, avoidDistance, mask);
    }

    public Vector3 FindCollisionAvoidDir(float radius, float avoidDistance, LayerMask mask)
    {
        // 使用全局方向列表
        foreach (var dir in BoidHelper.directions)
        {
            Vector3 worldDir = transform.TransformDirection(dir);
            if (!Physics.SphereCast(position, radius, worldDir, out _, avoidDistance, mask))
                return worldDir;
        }
        return forward; // 没找到就保持前进
    }
}

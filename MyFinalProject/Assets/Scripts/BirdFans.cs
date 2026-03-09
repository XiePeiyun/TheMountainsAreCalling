using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdFans : MonoBehaviour
{

    [Header("References")]
    public BirdsGroup birdsGroup;
    public GameObject birdFanPrefab;
    public Transform target;   // BirdKing
    public int totalBirds = 10;//从BirdsGroup获得的数量，UI text显示的数量
    public int maxRealBirds = 888;
    public int realBirds = 10;   //真实的birdfan数量

    [Header("Boid Parameters")]
    public float minSpeed = 2f;
    public float maxSpeed = 6f;
    public float maxSteerForce = 3f;

    public float perceptionRadius = 3f;
    public float avoidRadius = 1f;

    [Header("Weights")]
    public float alignWeight = 1f;
    public float cohesionWeight = 1f;
    public float separateWeight = 2f;
    public float followWeight = 1.2f;

    [Header("Obstacle Avoidance")]
    public float boundsRadius = 0.3f;
    public float collisionAvoidDst = 3f;
    public float avoidCollisionWeight = 10f;
    public LayerMask obstacleMask;

    private readonly List<Boid> boids = new List<Boid>();


    void Start()
    {
        // 如果绑定了 BirdsGroup，则订阅事件
        if (birdsGroup != null)
        {
            birdsGroup.OnTotalBirdsChanged += ReceiveTotalBirds;
            totalBirds = birdsGroup.totalBirds;  // 初始化同步
        }

        // 初始化 realBirds
        realBirds = Mathf.Min(totalBirds, maxRealBirds);
        SyncBirds();
    }

    void Update()
    {
        UpdateBoids();
    }

    // 事件回调：BirdsGroup 推送最新 totalBirds
    private void ReceiveTotalBirds(int newTotal)
    {
        totalBirds = newTotal;

        if (totalBirds < maxRealBirds)
            realBirds = totalBirds;     // 变少
        else
            realBirds = maxRealBirds;   // 保持上限值

        SyncBirds();

        Debug.Log($"[BirdFans] 接收 totalBirds = {totalBirds} | realBirds = {realBirds}");
    }

    // ---------------------------
    // Create / Remove Birds
    // ---------------------------
    public void AddFan()
    {
        if (boids.Count >= realBirds) return;
        CreateBoid();
    }

    public void RemoveFan()
    {
        if (boids.Count == 0) return;
        var b = boids[boids.Count - 1];
        boids.RemoveAt(boids.Count - 1);
        Destroy(b.gameObject);
    }

    public void SyncBirds()
    {
        // 注意这里同步针对 realBirds，而不是 totalBirds
        while (boids.Count < realBirds)
            AddFan();

        while (boids.Count > realBirds)
            RemoveFan();

    }

    private void CreateBoid()
    {
        Vector3 pos = target.position + Random.insideUnitSphere * 2f;
        GameObject obj = Instantiate(birdFanPrefab, pos, Quaternion.identity, transform);

        Boid boid = obj.GetComponent<Boid>();
        boid.position = obj.transform.position;
        boid.forward = obj.transform.forward;
        boid.speed = Random.Range(minSpeed, maxSpeed);

        boids.Add(boid);
    }


    // ---------------------------
    // Main Boid Update Loop
    // ---------------------------
    private void UpdateBoids()
    {
        if (boids.Count == 0) return;

        foreach (Boid boid in boids)
        {
            Vector3 align = Vector3.zero;
            Vector3 cohesion = Vector3.zero;
            Vector3 separation = Vector3.zero;
            int count = 0;

            foreach (Boid other in boids)
            {
                if (other == boid) continue;

                float sqrDist = (other.position - boid.position).sqrMagnitude;
                if (sqrDist < perceptionRadius * perceptionRadius)
                {
                    count++;
                    align += other.forward;
                    cohesion += other.position;

                    if (sqrDist < avoidRadius * avoidRadius)
                        separation += (boid.position - other.position);
                }
            }

            if (count > 0)
            {
                align /= count;
                cohesion /= count;
                cohesion = (cohesion - boid.position);
            }

            // Follow BirdKing
            Vector3 follow = (target.position - boid.position);

            // Final Acceleration
            Vector3 acceleration = Vector3.zero;
            acceleration += SteerTowards(boid, align) * alignWeight;
            acceleration += SteerTowards(boid, cohesion) * cohesionWeight;
            acceleration += SteerTowards(boid, separation) * separateWeight;
            acceleration += SteerTowards(boid, follow) * followWeight;

            // -------------- 避障（正确的位置） ----------------
            if (boid.IsHeadingForCollision(boundsRadius, collisionAvoidDst, obstacleMask))
            {
                Vector3 avoidDir = boid.FindCollisionAvoidDir(boundsRadius, collisionAvoidDst, obstacleMask);
                acceleration += SteerTowards(boid, avoidDir) * avoidCollisionWeight;
            }
            // ---------------------------------------------------

            boid.UpdateBoid(acceleration, minSpeed, maxSpeed);
        }
    }


    private Vector3 SteerTowards(Boid boid, Vector3 vector)
    {
        if (vector == Vector3.zero) return Vector3.zero;

        vector = vector.normalized * maxSpeed;
        Vector3 steer = vector - boid.forward * boid.speed;

        return Vector3.ClampMagnitude(steer, maxSteerForce);
    }



}

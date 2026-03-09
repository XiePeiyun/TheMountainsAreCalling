using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public int width = 20;
    public int height = 20;
    public float cellSize = 1f;

    private Dictionary<Vector2Int, GridCell> grid = new Dictionary<Vector2Int, GridCell>();


    private void Awake()
    {
        BuildIndexFromScene();
    }

    //在这里加一个* 路径搜索（BFS / A）**，
    //“绕开不能跨越的格子”，但允许“跨越但不能停留”的格子作为中转。
    public List<GridCell> FindPath(GridCell start, GridCell goal)
    {
        if (start == null || goal == null) return null;
        if (goal == start) return new List<GridCell> { start };

        Queue<GridCell> queue = new Queue<GridCell>();
        Dictionary<GridCell, GridCell> cameFrom = new Dictionary<GridCell, GridCell>();

        queue.Enqueue(start);
        cameFrom[start] = null;

        Vector2Int[] dirs = new Vector2Int[]
        {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
        };

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == goal) break;

            foreach (var d in dirs)
            {
                Vector2Int nextPos = current.gridPos + d;
                GridCell next = GetCell(nextPos);
                if (next == null) continue;

                // 条件：必须可通行或可跨越
                if (!next.isWalkable && !next.isCrossable) continue;

                if (!cameFrom.ContainsKey(next))
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        // 回溯路径
        if (!cameFrom.ContainsKey(goal)) return null; // 无法到达

        List<GridCell> path = new List<GridCell>();
        GridCell temp = goal;
        while (temp != null)
        {
            path.Add(temp);
            temp = cameFrom[temp];
        }
        path.Reverse();

        return path;
    }

    // 计算步数（考虑绕路）
    public int GetPathStepCount(GridCell start, GridCell goal)
    {
        var path = FindPath(start, goal);
        if (path == null) return int.MaxValue;
        return path.Count - 1; // 步数 = 格子数 - 1
    }


    // 从场景中已放置的 GridCell 实例建立索引（适合你手工摆放的地图）
    public void BuildIndexFromScene()
    {
        grid.Clear();
        GridCell[] all = FindObjectsOfType<GridCell>();
        foreach (var c in all)
        {
            // 以 world pos 的 x,z 四舍五入映射到整格（假设每格中心坐标是整数）
            // 如果你用了不同坐标系统，请相应地调整映射策略
            int gx = Mathf.RoundToInt(c.transform.position.x / cellSize);
            int gy = Mathf.RoundToInt(c.transform.position.z / cellSize);
            Vector2Int pos = new Vector2Int(gx, gy);

            c.SetGridPos(pos.x, pos.y);
            if (!grid.ContainsKey(pos))
                grid.Add(pos, c);
            else
                Debug.LogWarning($"重复的格子坐标 {pos} 在 {c.name}");
        }
    }

    // 返回指定坐标的格子（null 表示不存在）
    public GridCell GetCell(Vector2Int pos)
    {
        grid.TryGetValue(pos, out GridCell cell);
        return cell;
    }

    // 通过世界坐标（Raycast hit 点）获取格子
    public GridCell GetCellFromWorldPos(Vector3 worldPos)
    {
        int gx = Mathf.RoundToInt(worldPos.x / cellSize);
        int gy = Mathf.RoundToInt(worldPos.z / cellSize);
        return GetCell(new Vector2Int(gx, gy));
    }

    //获取两个格子的中心点距离（世界坐标距离）。
    public float GetDistance(GridCell a, GridCell b)
    {
        if (a == null || b == null) return float.PositiveInfinity;
        return Vector3.Distance(a.transform.position, b.transform.position);
    }


    //获取与目标格子在指定范围内的所有格子。
    public List<GridCell> GetCellsInRange(GridCell center, float range)
    {
        List<GridCell> list = new List<GridCell>();
        foreach (var kvp in grid)
        {
            float dist = Vector3.Distance(center.transform.position, kvp.Value.transform.position);
            if (dist <= range)
                list.Add(kvp.Value);
        }
        return list;
    }

}

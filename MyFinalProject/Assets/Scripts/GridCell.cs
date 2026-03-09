using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CellType
{
    Forest,     // Goal
    City,       // Road
    Desert,     // Block
    Mountain    // Crossing
}

public class GridCell : MonoBehaviour
{
    public CellType cellType;
    public Vector2Int gridPos;   // 格子坐标

    public bool isWalkable;
    public bool isSafe;        // 是否安全（是否会触发骰子）
    public bool isCrossable;   // 是否可以跨越（经过但不能停）


    private Renderer rend;
    private Color originalColor;
    private bool hasOriginal = false;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalColor = rend.material.color;
            hasOriginal = true;
        }

        ApplyCellTypeRules();

    }

    public void ApplyCellTypeRules()
    {
        switch (cellType)
        {
            case CellType.Forest:
                isWalkable = true;
                isSafe = true;
                isCrossable = true;
                break;
            case CellType.City:
                isWalkable = true;
                isSafe = false; // dice
                isCrossable = true;
                break;
            case CellType.Desert:
                isWalkable = false;
                isSafe = false;
                isCrossable = false;
                break;
            case CellType.Mountain:
                isWalkable = false;
                isSafe = true;
                isCrossable = true;
                break;
        }
    }



    public void SetGridPos(int x, int y)
    {
        gridPos = new Vector2Int(x, y);
    }

    // 高亮/取消高亮（color = null 表示恢复原色）
    public void SetHighlight(Color? color)
    {
        if (rend == null) return;
        if (color.HasValue)
            rend.material.color = color.Value;
        else if (hasOriginal)
            rend.material.color = originalColor;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    [Header("References")]
    public BirdKing birdKing; // 拖入 BirdKing 对象
    public Button moveButton; // 拖入自身按钮组件
    public Dice targetDice;        // Inspector 指定要销毁的 Dice

    [Header("状态")]
    public bool isPressed = false; // 当前是否已点击

    public void OnMoveClicked()
    {
        if (birdKing == null)
        {
            Debug.LogWarning("Move 按钮未绑定 BirdKing！");
            return;
        }

        isPressed = true;
        Debug.Log("Move 按钮被点击，通知 BirdKing。");

        birdKing.OnMoveButtonPressed(this);


        if (targetDice != null)
        {
            targetDice.DestroyDice();
        }
        else
        {
            Debug.LogWarning("Move 按钮未绑定目标 Dice！");
        }
    }

    public void ResetMoveButton()
    {
        isPressed = false;
        if (moveButton != null)
            moveButton.interactable = true;
    }

    public void DisableMoveButton()
    {
        if (moveButton != null)
            moveButton.interactable = false;
    }
}

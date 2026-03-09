using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EndFailGame : MonoBehaviour
{
    [Header("Reference")]
    public BirdsGroup birdsGroup;     // 指向你的 BirdsGroup
    public GameObject gameObject1;    // 失败界面或提示物体
    public GameObject gameObject2;    // 失败界面或提示物体

    private void Start()
    {
        if (gameObject1 != null)
            gameObject1.SetActive(false); // 开局隐藏
        if (gameObject2 != null)
            gameObject2.SetActive(false); // 开局隐藏


        if (birdsGroup != null)
        {
            // 订阅 BirdsGroup 的事件
            birdsGroup.OnTotalBirdsChanged += OnTotalBirdsChanged;

            // 初始化检查一次
            OnTotalBirdsChanged(birdsGroup.totalBirds);
        }
    }

    private void OnDestroy()
    {
        if (birdsGroup != null)
            birdsGroup.OnTotalBirdsChanged -= OnTotalBirdsChanged;
    }

    // 当 totalBirds 变化时，会自动调用这个函数
    private void OnTotalBirdsChanged(int total)
    {
        if (total <= 0)
        {
            Debug.Log("[EndFailGame] totalBirds = 0，显示失败界面");
            gameObject1?.SetActive(true);
        }
    }

    public void ActivateGameObject2()
    {
        if (gameObject2 != null)
            gameObject2.SetActive(true);
    }
}

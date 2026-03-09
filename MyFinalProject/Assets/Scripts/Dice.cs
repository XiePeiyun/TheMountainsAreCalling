using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public enum DiceType { Weather, Hunted, Disaster }
    public DiceType diceType = DiceType.Weather;
    //public DiceType diceType = DiceType.Hunted;
    //public DiceType diceType = DiceType.Disaster;

    private Rigidbody rb;
    private Collider diceCollider;
    private bool hasDropped = false;

    private bool hasChanged = false;
    private string resultName;


    [Header("Six Faces Sprites")]
    public Sprite[] faceSprites = new Sprite[6]; // 每个骰子只配置6张图片

    [Header("Six Faces GameObjects")]
    public GameObject[] faces = new GameObject[6];

 




    //[Header("Sprites")]
    //public Sprite sunnySprite;
    //public Sprite cloudSprite;
    //public Sprite windSprite;
    //public Sprite rainSprite;
    //public Sprite snowSprite;
    //public Sprite lightningSprite;


    //[Header("Hunted Sprites")]
    //public Sprite oneStarHunter;
    //public Sprite twoStarHunter;
    //public Sprite threeStarHunter;
    //public Sprite personalAssistance;
    //public Sprite groupRescue;
    //public Sprite socialAssistance;

    //[Header("Disaster Sprites")]
    //public Sprite lackForest;
    //public Sprite lightingPollution;
    //public Sprite highVoltage;
    //public Sprite infectPlague;
    //public Sprite niceHabitat;
    //public Sprite groupHealing;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        diceCollider = GetComponent<Collider>();

    }

    //设置结果
    public void SetResult(string name)
    {
        resultName = name;

    }



    // 落地后更新所有 faces 为对应的 Sprite
    private void UpdateFacesSprite()
    {
        if (faceSprites == null || faceSprites.Length != 6) return;
        if (faces == null || faces.Length != 6) return;

        // 找到匹配 resultName 的 Sprite
        Sprite targetSprite = faceSprites[0]; // 默认第一张
        foreach (var s in faceSprites)
        {
            if (s != null && s.name == resultName)
            {
                targetSprite = s;
                break;
            }
        }

        // 更新所有 faces 的 Sprite
        for (int i = 0; i < faces.Length; i++)
        {
            if (faces[i] == null) continue;

            // 支持 Image 或 SpriteRenderer
            var img = faces[i].GetComponent<Image>();
            if (img != null)
            {
                img.sprite = targetSprite;
            }
            else
            {
                var sr = faces[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = targetSprite;
            }
        }
    }



    public void DropDice()
    {
        if (hasDropped) return;
        hasDropped = true;

        // 可加力让骰子旋转
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        if (diceCollider != null)
            diceCollider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision collision)
    {


        if (hasChanged) return; // 防止重复触发
        hasChanged = true;

        // 碰撞时更新显示
        UpdateFacesSprite();

        // 可以添加音效或震动等效果





    }


    public void DestroyDice()
    {
        // 将 Collider 设置为 Trigger
        if (diceCollider != null)
            diceCollider.isTrigger = true;

        // 给 Rigidbody 一个向下速度，让它穿过地面
        if (rb != null)
            rb.velocity = Vector3.down * 3f;

        // 如果对象被禁用，协程无法运行，所以使用全局协程启动器
        if (gameObject.scene.IsValid()) // 确保是场景实例
            CoroutineRunner.Instance.StartCoroutine(DestroyAfterSecondsSafe(this, 1f));

    }

    // 安全协程：即使对象 inactive 也能延迟销毁
    private static IEnumerator DestroyAfterSecondsSafe(Dice dice, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (dice != null)
            Destroy(dice.gameObject);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DiceImage3 : MonoBehaviour
{
    public RawImage displayImage;     // 改成 RawImage
    public Texture defaultTexture;         // 默认静止图片

    public Texture[] birdKingTextures; // 改成 Texture 类型
    public float animationInterval = 0.5f;
    private Coroutine animationCoroutine;

    public void StartBirdKingAnimation()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PlayBirdKingAnimation());
    }

    public void StopBirdKingAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }


        if (displayImage != null && defaultTexture != null)
        {
            displayImage.texture = defaultTexture;
        }
    }

    private IEnumerator PlayBirdKingAnimation()
    {
        int index = 0;
        while (true)
        {
            if (birdKingTextures != null && birdKingTextures.Length > 0 && displayImage != null)
            {
                displayImage.texture = birdKingTextures[index]; // 使用 texture
                index = (index + 1) % birdKingTextures.Length;
            }
            yield return new WaitForSeconds(animationInterval);
        }
    }
}

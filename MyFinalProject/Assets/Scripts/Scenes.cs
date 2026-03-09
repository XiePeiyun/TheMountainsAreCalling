using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void GoToStartManu()
    {
        SceneManager.LoadScene(0);
    }
    public void GoToTutorial()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToMainGame()
    {
        SceneManager.LoadScene(2);

        // 延迟一帧保证场景加载完成再重置
        StartCoroutine(ResetStoryAndNextTurn());
    }

    public void GoToEnding()
    {
        SceneManager.LoadScene(3);
    }

    private IEnumerator ResetStoryAndNextTurn()
    {
        yield return null; // 等待一帧

        // 重置 NextTurn
        NextTurn nextTurn = FindObjectOfType<NextTurn>();
        if (nextTurn != null)
            nextTurn.currentTurn = 0;

        Story[] stories = FindObjectsOfType<Story>();
        foreach (var s in stories)
        {
            s.birdsTotal = 0;
            s.currentTurn = 0;
            s.RefreshAllButtons();
        }

        Debug.Log("场景切换完成，Story 和 NextTurn 已重置");
    }

}

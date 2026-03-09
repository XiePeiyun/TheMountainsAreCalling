using UnityEngine;

public class KeepLanguageAlive : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    public GameObject weatherUI;
    public GameObject huntedUI;
    public GameObject disasterUI;

    private GameObject currentActivePanel;

    void Awake()
    {
        Instance = this;
    }

    public void TogglePanel(GameObject panel)
    {
        if (currentActivePanel == panel)
        {
            panel.SetActive(false);
            currentActivePanel = null;
        }
        else
        {
            if (currentActivePanel != null)
                currentActivePanel.SetActive(false);

            panel.SetActive(true);
            currentActivePanel = panel;
        }
    }
}

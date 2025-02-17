using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameAchievePanel : MonoBehaviour
{
    [SerializeField] GameObject optionPanel;
    [SerializeField] GameObject achievePanel;
    [SerializeField] GameObject backgroundPanel;
    public void ExitBtn()
    {
        //backgroundPanel.SetActive(false);
        achievePanel.SetActive(false);
        optionPanel.SetActive(true);
    }
}

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
        if(!GameManager.Instance.gameClear) optionPanel.SetActive(true);
        else backgroundPanel.SetActive(false);
    }
}

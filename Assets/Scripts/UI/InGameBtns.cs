using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleasRecordBtn : MonoBehaviour
{
    
    [SerializeField] GameObject ClearRecordPanel;
    public void StartGame()
    {
        SceneManager.LoadScene("InGame");
    }
    public void HomeBtn()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void playAgainBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
    }
    public void ClearRecordBtn()
    {
        if (!ClearRecordPanel.activeSelf)
        {
            ClearRecordPanel.SetActive(true);
        }
    }
    public void ExitBtn()
    {
        if(ClearRecordPanel.activeSelf)
        {
            ClearRecordPanel.SetActive(false);
        }
    }
}

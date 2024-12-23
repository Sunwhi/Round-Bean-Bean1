using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CleasRecordBtn : MonoBehaviour
{
    
    [SerializeField] GameObject ClearRecordPanel;
    // 업적창 배경 어둡게 하기 위한 Panel
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] Button achieveBtn;
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
        if (!BackgroundPanel.activeSelf)
        {
            BackgroundPanel.SetActive(true);
        }
    }
    public void ExitBtn()
    {
        if(ClearRecordPanel.activeSelf)
        {
            ClearRecordPanel.SetActive(false);
        }
        if (BackgroundPanel.activeSelf)
        {
            BackgroundPanel.SetActive(false);
        }
    }

    private void Start()
    {
        float score = PlayerPrefs.GetFloat("score0");
        // 기록이 안 남아있을 시 버튼 흐리고 터치 x
        if (score == 10000f)
        {
            achieveBtn.interactable = false; // 버튼 터치 불가, 흐리게 처리
        }
        else
        {
            achieveBtn.interactable = true; // 버튼 터치 가능
        }
    }
}

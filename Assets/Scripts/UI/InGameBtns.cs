using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CleasRecordBtn : MonoBehaviour
{
    
    [SerializeField] GameObject ClearRecordPanel;
    // 업적창 배경, 옵션창 배경 어둡게 하기 위한 Panel
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] GameObject OptionPanel;
    [SerializeField] GameObject ConfirmPanel;
    [SerializeField] Button achieveBtn;
    [SerializeField] GameObject optionExitBtn;
    [SerializeField] GameObject soundBtn;
    [SerializeField] GameObject instructionPanel;
    [SerializeField] GameObject instructionExitBtn;
    //효과음
    public AudioClip optionClickClip;
    public void StartGame()
    {
        SceneManager.LoadScene("InGame");
    }
    public void ClearHomeBtn()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1.0f;
    }
    public void OptionHomeBtn()
    {
        ConfirmPanel.SetActive(true);
        optionExitBtn.SetActive(false);
        soundBtn.SetActive(false);

    }
    public void PlayAgainBtn()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("InGame");
    }
    public void ClearRecordBtn()
    {
        if (!ClearRecordPanel.activeSelf)
        {
            Time.timeScale = 0;
            if(!GameManager.Instance.gameClear) BackgroundPanel.SetActive(true);
            ClearRecordPanel.SetActive(true);
            OptionPanel.SetActive(false);
        }
    }
    public void ExitBtn()
    {
        if(ClearRecordPanel.activeSelf)
        {
            ClearRecordPanel.SetActive(false);
        }
    }
    public void OptionExitBtn()
    {
        BackgroundPanel.SetActive(false);
        OptionPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void OptionBtn()
    {
        SoundManager.Instance.SFXPlay("OptionClick", optionClickClip);
        OptionPanel.SetActive(true);
        BackgroundPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ConfirmYes()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1.0f;
    }
    public void ConfirmNo()
    {
        ConfirmPanel.SetActive(false);
        optionExitBtn.SetActive(true);
        soundBtn.SetActive(true);
    }
    
    public void InstructionBtn()
    {
        instructionPanel.SetActive(true);
    }
    public void InstructionExitBtn()
    {
        instructionPanel.SetActive(false);
    }
    void Update()
    {
        float score = PlayerPrefs.GetFloat("score0");
        // 기록이 안 남아있을 시 버튼 흐리고 터치 x
        if (score == 10000f && PlayerPrefs.GetFloat("recentScore") == 0)
        {
            achieveBtn.interactable = false; // 버튼 터치 불가, 흐리게 처리
        }
        else
        {
            achieveBtn.interactable = true; // 버튼 터치 가능
        }
    }
}

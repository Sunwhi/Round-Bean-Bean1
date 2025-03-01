using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CleasRecordBtn : MonoBehaviour
{
    
    [SerializeField] GameObject ClearRecordPanel;
    // ����â ���, �ɼ�â ��� ��Ӱ� �ϱ� ���� Panel
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] GameObject OptionPanel;
    [SerializeField] GameObject ConfirmPanel;
    [SerializeField] Button achieveBtn;
    [SerializeField] GameObject optionExitBtn;
    [SerializeField] GameObject soundBtn;
    [SerializeField] GameObject instructionPanel;
    [SerializeField] GameObject instructionExitBtn;
    //ȿ����
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
        // ����� �� �������� �� ��ư �帮�� ��ġ x
        if (score == 10000f && PlayerPrefs.GetFloat("recentScore") == 0)
        {
            achieveBtn.interactable = false; // ��ư ��ġ �Ұ�, �帮�� ó��
        }
        else
        {
            achieveBtn.interactable = true; // ��ư ��ġ ����
        }
    }
}

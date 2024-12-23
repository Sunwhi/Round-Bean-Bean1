using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CleasRecordBtn : MonoBehaviour
{
    
    [SerializeField] GameObject ClearRecordPanel;
    // ����â ��� ��Ӱ� �ϱ� ���� Panel
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
        // ����� �� �������� �� ��ư �帮�� ��ġ x
        if (score == 10000f)
        {
            achieveBtn.interactable = false; // ��ư ��ġ �Ұ�, �帮�� ó��
        }
        else
        {
            achieveBtn.interactable = true; // ��ư ��ġ ����
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBtn : MonoBehaviour
{
    [SerializeField] GameObject AchievePanel;
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] Button achieveBtn;
    [SerializeField] GameObject instructionPanel;
    [SerializeField] GameObject creditPanel;
    public void AchieveBtn()
    {
        if (!AchievePanel.activeSelf)
        {
            AchievePanel.SetActive(true);
        }
        if (!BackgroundPanel.activeSelf)
        {
            BackgroundPanel.SetActive(true);
        }
    }
    public void ExitBtn()
    {
        if (AchievePanel.activeSelf)
        {
            AchievePanel.SetActive(false);
        }
        if (BackgroundPanel.activeSelf)
        {
            BackgroundPanel.SetActive(false);
        }

        if(instructionPanel.activeSelf)
            instructionPanel.SetActive(false);
         
        if(creditPanel.activeSelf)
            creditPanel.SetActive(false);
    }
    public void InstructionBtn()
    {
        if (!instructionPanel.activeSelf)
            instructionPanel.SetActive(true);
    }
    public void CreditBtn()
    {
        if(!creditPanel.activeSelf)
            creditPanel.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
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
    // && PlayerPrefs.GetFloat("recentScore") == 0
    // Update is called once per frame
    void Update()
    {
        
    }
}

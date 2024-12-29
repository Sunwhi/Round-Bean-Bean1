using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuBtn : MonoBehaviour
{
    [SerializeField] GameObject AchievePanel;
    [SerializeField] GameObject BackgroundPanel;
    [SerializeField] Button achieveBtn;
    public void ClearRecordBtn()
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
    }
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        
    }
}

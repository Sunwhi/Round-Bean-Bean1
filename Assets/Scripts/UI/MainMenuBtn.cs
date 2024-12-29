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

    // Update is called once per frame
    void Update()
    {
        
    }
}

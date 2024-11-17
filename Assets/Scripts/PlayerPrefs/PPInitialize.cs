using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTEST : MonoBehaviour
{
    float score;
    string strScore;
    string scoreNum;
    string dateNum;
    // Start is called before the first frame update
    void Start()
    {
        // Initialized Ű�� �̿��Ͽ� �ʱ�ȭ ���� ������. ���� ������ ó�� �� �����̶�� �ڵ����� �ʱ�ȭ�� ��.
        if(PlayerPrefs.HasKey("Initialized"))
        {
            Debug.Log("Already initialized");
        }
        else
        {
            PlayerPrefsInitialize();
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        // P ������ ��� Ȯ�� ����
        if(Input.GetKeyUp(KeyCode.P))
        {
            for(int i = 0; i < 5; i++)
            {
                scoreNum = "score" + i;
                score = PlayerPrefs.GetFloat(scoreNum);
                
                strScore = FormatTime(score);
                Debug.Log(scoreNum + " : " + strScore);

                dateNum = "date" + i;
                Debug.Log("date : " + PlayerPrefs.GetString(dateNum));
            }
        }
        // I ������ �ʱ�ȭ
        if(Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefsInitialize();
        }
    }

    // PlayerPrefs �ʱ�ȭ. ���� ����� �� �ʱ� ���� Ȯ�� ���ؼ�.
    void PlayerPrefsInitialize()
    {
        for (int i = 0; i < 5; i++)
        {
            scoreNum = "score" + i;
            PlayerPrefs.SetFloat(scoreNum, 10000f); // �ʱ�ȭ ���ڴ� 10000f
            dateNum = "date" + 0;
            PlayerPrefs.SetString(dateNum, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        PlayerPrefs.SetInt("Initialized", 1);

        Debug.Log("PlayerPrefs are newly Initialized");
    }

    // ��� ���ڰ� ǥ�� 00 : 00
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    
}

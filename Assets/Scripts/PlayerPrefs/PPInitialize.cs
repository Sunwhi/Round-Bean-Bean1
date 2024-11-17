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
        // Initialized 키를 이용하여 초기화 여부 결정함. 만약 게임을 처음 깐 시점이라면 자동으로 초기화가 됨.
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
        // P 누르면 기록 확인 가능
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
        // I 누르면 초기화
        if(Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefsInitialize();
        }
    }

    // PlayerPrefs 초기화. 게임 깔았을 때 초기 상태 확인 위해서.
    void PlayerPrefsInitialize()
    {
        for (int i = 0; i < 5; i++)
        {
            scoreNum = "score" + i;
            PlayerPrefs.SetFloat(scoreNum, 10000f); // 초기화 숫자는 10000f
            dateNum = "date" + 0;
            PlayerPrefs.SetString(dateNum, DateTime.Now.ToString("yyyy-MM-dd"));
        }
        PlayerPrefs.SetInt("Initialized", 1);

        Debug.Log("PlayerPrefs are newly Initialized");
    }

    // 기록 예쁘게 표시 00 : 00
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    
}

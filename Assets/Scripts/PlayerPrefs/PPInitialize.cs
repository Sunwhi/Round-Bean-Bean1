using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTEST : MonoBehaviour
{
    float score;
    string strScore;
    string scoreNum;
    // Start is called before the first frame update
    void Start()
    {
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
        if(Input.GetKeyUp(KeyCode.P))
        {
            for(int i = 0; i < 5; i++)
            {
                scoreNum = "score" + i;
                score = PlayerPrefs.GetFloat(scoreNum);
                
                strScore = FormatTime(score);
                Debug.Log(scoreNum + " : " + strScore);
            }
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefsInitialize();
        }
    }



    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    void PlayerPrefsInitialize()
    {
        for (int i = 0; i < 5; i++)
        {
            scoreNum = "score" + i;
            PlayerPrefs.SetFloat(scoreNum, 10000f);
        }
        PlayerPrefs.SetInt("Initialized", 1);

        Debug.Log("PlayerPrefs are newly Initialized");
    }
}

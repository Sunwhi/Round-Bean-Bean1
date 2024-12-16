using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PPConfirm : MonoBehaviour
{
    float score;
    string strScore;
    string scoreNum;
    string dateNum;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            for (int i = 0; i < 10; i++)
            {
                scoreNum = "score" + i;
                score = PlayerPrefs.GetFloat(scoreNum);

                strScore = GameManager.Instance.FormatTime(score);
                Debug.Log(scoreNum + " : " + strScore);

                dateNum = "date" + i;
                Debug.Log(PlayerPrefs.GetString(dateNum));
            }
        }
    }
}

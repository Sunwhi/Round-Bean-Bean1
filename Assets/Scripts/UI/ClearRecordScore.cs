using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClearRecordPanel : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private string score;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach(Transform child in transform)
        {
            scoreText = child.GetComponent<TextMeshProUGUI>();

            score = "score" + i;
            if(PlayerPrefs.GetFloat(score) == 10000)
            {
                scoreText.text = "";
            }
            else
            {
                scoreText.text = FormatTime(PlayerPrefs.GetFloat(score));
            }
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        int milliSeconds = Mathf.RoundToInt(time * 100 % 100);
        return $"{minutes:00} : {remainSeconds:00} : {milliSeconds:00}";
    }
}

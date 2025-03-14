using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
//using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class AchieveGameOverScore : MonoBehaviour
{
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI dateText;
    private string score;
    private string date;

    [SerializeField] Image seasonClockImg;
    [SerializeField] Sprite clockAutumn;
    [SerializeField] Sprite clockWinter;
    [SerializeField] Sprite clockSpring;
    [SerializeField] Sprite clockSummer;

    private int gameOverSeason;
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            gameOverSeason = PlayerPrefs.GetInt("recentSeason");

            if (PlayerPrefs.GetFloat("recentScore") != 0)
            {
                switch (gameOverSeason)
                {
                    case 0:
                        seasonClockImg.sprite = clockSpring;
                        break;
                    case 1:
                        seasonClockImg.sprite = clockSummer;
                        break;
                    case 2:
                        seasonClockImg.sprite = clockAutumn;
                        break;
                    case 3:
                        seasonClockImg.sprite = clockWinter;
                        break;
                }
            }
        }
        catch(NullReferenceException)
        {
            Debug.Log("not yet");
        }
        
        foreach(Transform child in transform)
        {
            if(child.gameObject.name == "Score")
            {
                scoreText = child.GetComponent<TextMeshProUGUI>();
                if(PlayerPrefs.GetFloat("recentScore") == 0)
                {
                    scoreText.text = "";
                }
                else
                {
                    scoreText.text = FormatTime(PlayerPrefs.GetFloat("recentScore"));
                }
            }
            else if(child.gameObject.name == "Date")
            {
                dateText = child.GetComponent<TextMeshProUGUI>();
                if(PlayerPrefs.GetFloat("recentScore") == 0)
                {
                    dateText.text = "";
                }
                else
                {
                    dateText.text = PlayerPrefs.GetString("recentDate");
                }
            }
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

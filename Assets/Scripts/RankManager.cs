using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RankManager : MonoBehaviour
{
    public static RankManager instance;
    public Text scoreText, myrank;
    public Text[] rank = new Text[5];
    public int score = 0;
    private int[] bestScore = new int[5];
    private string[] bestName = new string[5];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ScoreSet(int currentScore, string currentName)
    {
        PlayerPrefs.SetString("CurrentPlayerName", currentName);
        PlayerPrefs.SetInt("CurrentPlayerScore", currentScore);

        int tmpScore = 0;
        string tmpName = "";

        for (int i = 0; i < 5; i++)
        {
            bestScore[i] = PlayerPrefs.GetInt(i + "BestScore");
            bestName[i] = PlayerPrefs.GetString(i + "BestName");

            while (bestScore[i] < currentScore)
            {
                tmpScore = bestScore[i];
                tmpName = bestName[i];
                bestScore[i] = currentScore;
                bestName[i] = currentName;

                PlayerPrefs.SetInt(i + "BestScore", currentScore);
                PlayerPrefs.SetString(i + "BestName", currentName);
                currentScore = tmpScore;
                currentName = tmpName;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            PlayerPrefs.SetInt(i + "BestScore", bestScore[i]);
            PlayerPrefs.SetString(i + "BestName", bestName[i]);
            Debug.Log("[RANK] " + PlayerPrefs.GetString(i + "BestName") + PlayerPrefs.GetInt(i + "BestScore"));
            rank[i].text = bestName[i] + " " + bestScore[i].ToString();
        }
    }
}

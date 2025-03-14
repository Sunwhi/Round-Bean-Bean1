using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClearRecordDate : MonoBehaviour
{
    private TextMeshProUGUI dateText;
    private string date;
    private string score;
    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            dateText = child.GetComponent<TextMeshProUGUI>();

            score = "score" + i;
            date = "date" + i;
            if(PlayerPrefs.GetFloat(score) == 10000)
            {
                dateText.text = "";
            }
            else
            {
                dateText.text = PlayerPrefs.GetString(date);
            }
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

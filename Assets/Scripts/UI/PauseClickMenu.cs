using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseClickMenu : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject menuPopup;
    void Update()
    {
        if (isGamePaused = true && menuPopup !=null && Input.GetKeyDown(KeyCode.Space))
        {
            ResumeGame();
            
        }
    }


    public void OnMenuButtonClick()
    {
        isGamePaused = true;
        PauseGame();
    }


    private void PauseGame()
    {
        Time.timeScale = 0.00000001f;
        Debug.Log("PauseGame");
    }


    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        Debug.Log("ResumeGame");
    }
}
   /* public void OnClickMenu()
    {
        Time.timeScale = 0f;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1f;
        }

    }*/


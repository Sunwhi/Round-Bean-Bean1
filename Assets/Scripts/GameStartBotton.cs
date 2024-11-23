using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameStartBotton : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("InGame");
    }
    public void HomeBtn()
    {
        SceneManager.LoadScene("MainScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameStartBotton : MonoBehaviour
{
    public AudioClip startBtnClickClip;
    public void StartGame()
    {
        //SoundManager.Instance.SFXPlay("StartBtnClick", startBtnClickClip);
        SceneManager.LoadScene("InGame");
    }
    public void HomeBtn()
    {
        SceneManager.LoadScene("MainScene");
    }
}

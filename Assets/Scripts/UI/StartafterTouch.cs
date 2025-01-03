using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartafterTouch : MonoBehaviour
{
    private bool isGameStarted = false; // 게임이 시작되었는지 여부

    private void Start()
    {
        Time.timeScale = 0f; // 게임 시작 시 정지 상태
        isGameStarted = false;
    }

    private void Update()
    {
        // 터치 감지 후 게임 시작
        if (!isGameStarted && Input.touchCount > 0)
        {
            StartGame();
        }
        // 데스크톱 환경에서 테스트용으로 마우스 클릭도 추가
        else if (!isGameStarted && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f; // 게임 시간 흐름 시작
        isGameStarted = true; // 게임 상태를 시작으로 설정
    }
}


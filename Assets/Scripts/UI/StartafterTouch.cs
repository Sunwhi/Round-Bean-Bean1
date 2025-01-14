using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartafterTouch : MonoBehaviour
{
    private bool isGameStarted = false; // 게임이 시작되었는지 여부
    public Button restartButton; // 다시하기 버튼
    private SoundManager soundManager; // 현재 씬에서의 SoundManager 참조

    private void Start()
    {
        Time.timeScale = 0.000001f; // 게임 시작 시 정지 상태
        isGameStarted = false;

        // 현재 씬에서 SoundManager 찾기
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("SoundManager not found in the current scene.");
        }

        // 다시하기 버튼에 클릭 이벤트 추가
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void Update()
    {
        // 터치 또는 클릭 감지 후 게임 시작
        if (!isGameStarted && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f; // 게임 시간 흐름 시작
        isGameStarted = true; // 게임 상태를 시작으로 설정

        // SoundManager가 존재할 경우 음원 재생 재개
        if (soundManager != null)
        {
            soundManager.ResumeAllSounds();
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 0.000001f; // 게임 시간 정지
        isGameStarted = false; // 게임 상태를 멈춤으로 설정

        // SoundManager가 존재할 경우 모든 음원 멈춤
        if (soundManager != null)
        {
            soundManager.StopAllSounds();
        }
    }
}


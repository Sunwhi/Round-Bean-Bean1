using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartafterTouch : MonoBehaviour
{
    private bool isGameStarted = false; // ������ ���۵Ǿ����� ����
    public Button restartButton; // �ٽ��ϱ� ��ư
    private SoundManager soundManager; // ���� �������� SoundManager ����

    private void Start()
    {
        Time.timeScale = 0.000001f; // ���� ���� �� ���� ����
        isGameStarted = false;

        // ���� ������ SoundManager ã��
        soundManager = FindObjectOfType<SoundManager>();
        if (soundManager == null)
        {
            Debug.LogWarning("SoundManager not found in the current scene.");
        }

        // �ٽ��ϱ� ��ư�� Ŭ�� �̺�Ʈ �߰�
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void Update()
    {
        // ��ġ �Ǵ� Ŭ�� ���� �� ���� ����
        if (!isGameStarted && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f; // ���� �ð� �帧 ����
        isGameStarted = true; // ���� ���¸� �������� ����

        // SoundManager�� ������ ��� ���� ��� �簳
        if (soundManager != null)
        {
            soundManager.ResumeAllSounds();
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 0.000001f; // ���� �ð� ����
        isGameStarted = false; // ���� ���¸� �������� ����

        // SoundManager�� ������ ��� ��� ���� ����
        if (soundManager != null)
        {
            soundManager.StopAllSounds();
        }
    }
}


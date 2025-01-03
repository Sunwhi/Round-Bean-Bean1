using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartafterTouch : MonoBehaviour
{
    private bool isGameStarted = false; // ������ ���۵Ǿ����� ����

    private void Start()
    {
        Time.timeScale = 0f; // ���� ���� �� ���� ����
        isGameStarted = false;
    }

    private void Update()
    {
        // ��ġ ���� �� ���� ����
        if (!isGameStarted && Input.touchCount > 0)
        {
            StartGame();
        }
        // ����ũ�� ȯ�濡�� �׽�Ʈ������ ���콺 Ŭ���� �߰�
        else if (!isGameStarted && Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        Time.timeScale = 1f; // ���� �ð� �帧 ����
        isGameStarted = true; // ���� ���¸� �������� ����
    }
}


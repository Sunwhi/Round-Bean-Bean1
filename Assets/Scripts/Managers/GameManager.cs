using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
/*
 * GameManager Ŭ���� 
 * �̱��� �̿��Ͽ� �ٸ� ���̳� �ٸ� ������Ʈ���� GameManager Ŭ���� �� �Լ��� ���� ���� ����!
 */
public class GameManager : MonoBehaviour
{
    [SerializeField] Collider2D animalCollider;
    public bool isGround = false;
    private bool gameOver;

    public TextMeshProUGUI textTime;  // �ð��� ��Ÿ���� �ؽ�Ʈ

    public static GameManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ���� �ٲ� ����
        }
        else
        {
            Destroy(gameObject); // �̹� GameManager�� �����ϸ� ���� ������ ������Ʈ�� ����
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // AnimalCollision.cs���� isGround�� true�� �Ǿ��ٸ� GameOver�Լ��� ȣ���Ѵ�.
        if (isGround) GameOver();

        //textTime�� ���� �ð��� ǥ��
        textTime.text = FormatTime(Time.time);
    }

    // �� ������ time�� �а� �ʷ� ����� ǥ����
    string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }
    // ������ �����.
    private void GameOver()
    {
        Time.timeScale = 0;
        gameOver = true;
    }
}

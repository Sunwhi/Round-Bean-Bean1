using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtnSound : MonoBehaviour
{
    private static StartBtnSound instance;
    private int playSound;
    public AudioClip startBtnSound;
    public AudioSource audioSource;
    void Awake()
    {
        playSound = 0;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� �Ѿ�� �������� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "InGame" && playSound == 0)
        {
            audioSource.clip = startBtnSound;
            audioSource.Play();
            playSound = 1;
        }
        if (SceneManager.GetActiveScene().name == "MainScene") playSound = 0;
    }
}

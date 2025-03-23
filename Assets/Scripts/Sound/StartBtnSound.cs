using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtnSound : MonoBehaviour
{
    public static StartBtnSound Instance { get; private set; }
    //����ȭ���� start��ư�� ���ؼ� �ΰ������� ���������� ȿ���� ������ �ϱ� ���� ���� playSound ����
    private int playSound;
    public AudioClip startBtnSound;
    public AudioSource audioSource;
    void Awake()
    {
        playSound = 0;

        if (Instance == null)
        {
            Instance = this;
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
        // ����ȭ�� -> �ΰ��ӽÿ��� �ѹ� ���
        if (SceneManager.GetActiveScene().name == "InGame" && playSound == 0)
        {
            audioSource.clip = startBtnSound;
            audioSource.Play();
            playSound = 1; // �ΰ��Ӿ������� �� �̻� ���ǹ��� ������� �ʴ´�.
        }
        // ����ȭ�� ���ư��� �ٽ� playSound = 0
        if (SceneManager.GetActiveScene().name == "MainScene") playSound = 0;
    }
}

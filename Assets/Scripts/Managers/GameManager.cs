using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Properties;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
/*
 * GameManager Ŭ���� 
 * �̱��� �̿��Ͽ� �ٸ� ���̳� �ٸ� ������Ʈ���� GameManager Ŭ���� �� �Լ��� ���� ���� ����!
 */
public class GameManager : MonoBehaviour
{
    Scene inGameScene;

    [SerializeField] Collider2D animalCollider;
    public bool gameOver;
    int onlyOnce = 0; // ���� ������ �� �ѹ��� ��� �����ϱ� ���� ���� �ӽú���

    float time;
    float finalScore; // ���� �ð� ���
    [SerializeField] TextMeshProUGUI textTime;  // �ð��� ��Ÿ���� �ؽ�Ʈ
    [SerializeField] GameObject textGameOver; // ���ӿ��� �ؽ�Ʈ, setActive() ȣ�� ���ؼ� GameObject�� 

    [SerializeField] GameObject animal;
    [SerializeField] Rigidbody2D frameRigidBody;
    public GameObject wheel;
    private UnicycleController unicycleController;
    private PlayerDragMovement playerDragMovement;

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
        inGameScene = SceneManager.GetActiveScene();

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;

        //textTime�� ���� �ð��� ǥ��
        if (!gameOver)
            textTime.text = FormatTime(time);

        // AnimalCollision.cs���� isGround�� true�� �Ǿ��ٸ� GameOver�Լ��� ȣ���Ѵ�.
        if (gameOver) GameOver();
    }

    // �� ������ time�� �а� �ʷ� ����� ǥ����
    // 00:00 (��, ��)�� ǥ���.
    string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    // ���ӿ����ÿ� �������� �ۿ� ����
    private void GameOver()
    {
        // ������ �� ������ �� �ѹ��� finalScore�� ����
        if(onlyOnce == 0)
        {
            finalScore = time;
            onlyOnce = 1;
            Debug.Log(FormatTime(finalScore));
        }


        // �÷��̾��������� 00:00�������� ���ھ� ���� -> ����ȭ�鿡�� ���� -> �迭 ���� �����ϰ� 10�� ��ϸ� ����� ������ �ڸ��ٴ� �������� ����
        PlayerPrefs.SetString("score", FormatTime(finalScore)); 

        Time.timeScale = 1f; // �ð� �ణ ������
        textGameOver.SetActive(true); // ���ӿ��� �ؽ�Ʈ



        /* ���� �����ſ��� �и���Ű��, �̵��������� �������� ������ */
        animal.transform.SetParent(null);

        animal.AddComponent<Rigidbody2D>();
        Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();

        if(frameRigidBody.angularVelocity > 0)
        {
            animalRigidBody.AddTorque(20f * Time.deltaTime);
        }
        else
        {
            animalRigidBody.AddTorque(-20f * Time.deltaTime);
        }
        // �̵����� ��ũ��Ʈ ���Ƽ� ���ӿ��� ���� �������� ���ϰ�
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;

        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }
}

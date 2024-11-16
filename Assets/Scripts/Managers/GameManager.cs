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
    Scene inGameScene; // ���� �ΰ��� �� ����

    [SerializeField] Collider2D animalCollider;
    public bool gameOver = false;
    int onlyOnce; // ���� ������ �� �ѹ��� ��� �����ϱ� ���� ���� �ӽú���

    float time; // ���� �ð� ���
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
            //DontDestroyOnLoad(gameObject); // ���� �ٲ� ���� -> �� �����(���� �����)�ÿ� �������� �ʱ�ȭ���� �ʴ� ������ �߻�! DontDestroyOnLoad�� ��� ���°ɷ�
        }
        else
        {
            Destroy(gameObject); // �̹� GameManager�� �����ϸ� ���� ������ ������Ʈ�� ����
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        inGameScene = SceneManager.GetActiveScene(); // ���� �۵��ǰ� �ִ� InGame���� ������

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0; // �� �Ǹ��� �ð� 0���� �ʱ�ȭ
        onlyOnce = 0; // �� �Ǹ��� 0���� �ʱ�ȭ

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
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    // ���ӿ����ÿ� �������� �ۿ� ����
    private void GameOver()
    {
        // ������ �� ������ �� �ѹ��� finalScore�� ����, �� �ѹ��� PlayerPrefs ��� ���Ӱ� ����
        if(onlyOnce == 0)
        {
            finalScore = time;
            ScoreSorting(finalScore);

            onlyOnce = 1;
            //Debug.Log(FormatTime(finalScore));
        }

        

        // �÷��̾��������� 43.4431s �������� ���ھ� ���� -> ����ȭ�鿡�� ���� -> �迭 ���� �����ϰ� 5�� ��ϸ� ����� ������ �ڸ��ٴ� �������� ����
        //PlayerPrefs.SetFloat("score", finalScore); 

        Time.timeScale = 1f; // �ð� �ణ ������
        textGameOver.SetActive(true); // ���ӿ��� �ؽ�Ʈ



        /* ���� �����ſ��� �и���Ű��, �̵��������� �������� ������ */
        animal.transform.SetParent(null);

        if(animal.GetComponent<Rigidbody2D>() == null)
        {
            animal.AddComponent<Rigidbody2D>();
            Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();

            if (frameRigidBody.angularVelocity > 0)
            {
                animalRigidBody.AddTorque(20f * Time.deltaTime);
            }
            else
            {
                animalRigidBody.AddTorque(-20f * Time.deltaTime);
            }
        }

        // �̵����� ��ũ��Ʈ ���Ƽ� ���ӿ��� ���� �������� ���ϰ� ��
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;


        //Debug.Log("�������");
        // ���� �̰� Update�� �ȿ� �����ϱ� ��� �����. �׷��� pp ����� �� �������°���
        //ScoreSorting(finalScore); // ��� PlayerPrefs�� ����



        // ���� ����� -> ���� ���� �ٽ� �ε��Ѵ�.
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    /*
     * ���ھ ������ PlayerPrefs�� ���� ���ھ���� ������ ���� ���Ӱ� ����Ѵ�.
     * Ŭ��� ���!
     */
    private void ScoreSorting(float finalScore)
    {
        int newScoreIdx = 0; // ���ο� ����� ����� Index
        string scoreNum; // PlayerPrefs�� ������ string
        string frontScoreNum; // ���ο� ��� ���������� scoreNum�� �� ����Ű��

        // ���ο� ����� ���° ����ΰ� ã��
        for(int i=0; i<5; i++)
        {
            scoreNum = "score" + i;

            if(finalScore < PlayerPrefs.GetFloat(scoreNum))
            {
                //Debug.Log("final : " + finalScore);
               // Debug.Log(scoreNum + " : " + PlayerPrefs.GetFloat(scoreNum));
                newScoreIdx = i;
               // Debug.Log(newScoreIdx);
                break;
            }
        }

        // ���ο� ��� PlayerPrefs�� �����ֱ�
        for(int i = 4; i>=newScoreIdx; i--)
        {
            scoreNum = "score" + i;

            if (i == newScoreIdx)
            {
                PlayerPrefs.SetFloat(scoreNum, finalScore);
                return;
            }

            frontScoreNum = "score" + (i - 1);

            PlayerPrefs.SetFloat(scoreNum,PlayerPrefs.GetFloat(frontScoreNum)); // ���� score�� ������ �ѱ��
        }
    }
}

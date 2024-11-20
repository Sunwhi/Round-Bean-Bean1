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
    public bool gameClear = false;
    int callFinalScoreOnce; // ���� ������ �� �ѹ��� ��� �����ϱ� ���� ���� �ӽú���
    // GameOver(), GameClear() �Լ� �� �ѹ����� ȣ���ϱ� ���� ����
    //bool isGameOverCalled = false; 
    //bool isGameClearCalled = false;

    float time; // ���� �ð� ���
    float finalScore; // ���� �ð� ���
    [SerializeField] TextMeshProUGUI textTime;  // �ð��� ��Ÿ���� �ؽ�Ʈ
    [SerializeField] GameObject textGameOver; // ���ӿ��� �ؽ�Ʈ, setActive() ȣ�� ���ؼ� GameObject�� 
    [SerializeField] GameObject textGameClear; // ���� Ŭ���� �ؽ�Ʈ

    [SerializeField] GameObject animal;
    [SerializeField] Rigidbody2D frameRigidBody;
    public GameObject wheel;
    public GameObject frame;
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


    void Start()
    {
        inGameScene = SceneManager.GetActiveScene(); // ���� �۵��ǰ� �ִ� InGame���� ������

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0; // �� �Ǹ��� �ð� 0���� �ʱ�ȭ
        callFinalScoreOnce = 0; // �� �Ǹ��� 0���� �ʱ�ȭ
    }


    void Update()
    {

        time += Time.deltaTime;

        //textTime�� ���� �ð��� ǥ��
        if (!gameOver && !gameClear)
            textTime.text = FormatTime(time);

        // AnimalCollision.cs���� isGround�� true�� �Ǿ��ٸ� GameOver�Լ��� ȣ���Ѵ�.
        // isGameOverCalled�� GameOver()�Լ��� �ѹ��� ����ǵ��� ��. �ƴϸ� ��� ȣ���. -> ��� ���
        if (gameOver)
        {
            GameOver();
        }
        // isGameClearCalled�� GameClear()�Լ��� �ѹ��� ����ǵ��� ��. �ƴϸ� ��� ȣ���.
        if (gameClear)
        {
            GameClear();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    /* �� ������ time�� �а� �ʷ� ����� ǥ����
     * 00:00 (��, ��)�� ǥ���.
    */
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    /*
     * ���ӿ����� �������� �ۿ� ������ �Լ�
     */
    private void GameOver()
    {
        // ������ �� ������ �� �ѹ��� finalScore�� ���� / �� �ѹ��� PlayerPrefs ��� ���Ӱ� ����
        // �̰� ���ϸ� Update�� ��� ȣ��Ǿ� ��� ��ϵ��� ������.
        if(callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            callFinalScoreOnce = 1;
        }

        
        Time.timeScale = 1f; // �ð� �ణ ������ -> �ƴ�
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


        // 'R' ���� ���� ����� -> ���� ���� �ٽ� �ε��Ѵ�.
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    private void GameClear()
    {
        // ������ �� ������ �� �ѹ��� finalScore�� ���� / �� �ѹ��� PlayerPrefs ��� ���Ӱ� ����
        // �̰� ���ϸ� Update�� ��� ȣ��Ǿ� ��� ��ϵ��� ������.
        if (callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            callFinalScoreOnce = 1;
        }


        Time.timeScale = 1f; // �ð� �ణ ������ -> �ƴ�
        textGameClear.SetActive(true); // ���� Ŭ���� �ؽ�Ʈ

        // �̵����� ��ũ��Ʈ ���Ƽ� ���ӿ��� ���� �������� ���ϰ� ��
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;


        // ���� ��������, ������ ���� ����?
        //animal.transform.SetParent(null); // �ܹ������� ������ �����ҰŴϱ� �и�?
        frameRigidBody.freezeRotation = true;


        Vector2 v = new Vector2(-0.1f, 0);
        if(frameRigidBody.velocity.magnitude <0.00001)
        {
            frameRigidBody.AddForce(v, ForceMode2D.Force);
            Debug.Log(frameRigidBody.velocity.magnitude);
        }
        // ���� �ӵ��� 0�� �Ǹ� ������ ���� 90���� �����.
        if (frameRigidBody.velocity.magnitude == 0)
        {
            Time.timeScale = 0;
            frameRigidBody.freezeRotation = false;
            if(frameRigidBody.transform.rotation.z > 0)
            {
                frameRigidBody.AddTorque(-100f * Time.deltaTime);
            }
            else
            {
                frameRigidBody.AddTorque(100f * Time.deltaTime);
            }
        }
        // 'R' ���� ���� ����� -> ���� ���� �ٽ� �ε��Ѵ�.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    /*
     * ���ھ ������ PlayerPrefs�� ���� ���ھ���� ������ ���� ���Ӱ� ����Ѵ�. 
     * ��, ���ĵǾ� ��ϵ��� ����.
     * ���߿� Ŭ���� ������ �� ���!
     */
    private void RecordNewScore(float finalScore)
    {
        int newScoreIdx = 0; // ���ο� ����� ����� Index
        string scoreNum; // PlayerPrefs�� ������ string
        string frontScoreNum; // ���ο� ��� ���������� scoreNum�� �� ����Ű��

        string dateNum;
        string frontDateNum;

        // ���ο� ����� ���° ����ΰ� ã��
        for(int i=0; i<5; i++)
        {
            scoreNum = "score" + i;

            if(finalScore < PlayerPrefs.GetFloat(scoreNum))
            { 
                newScoreIdx = i;
                break;
            }
            if (i == 4) return; // 5���� ��ϵ鿡 ������ ������ �ƿ� ��� X �׸��� return
        }

        // ���ο� ��� PlayerPrefs�� newScoreIdx�� �����ֱ�
        // �ڿ������� ������. �ڿ������� ��ĭ ���� ��ϵ��� �ڷ� �̵�
        for(int i = 4; i>=newScoreIdx; i--)
        {
            scoreNum = "score" + i;
            dateNum = "date" + i;

            if (i == newScoreIdx) // ���� ���Ӱ� ��ϵ� index��� ����ϰ� Ƣ��
            {
                PlayerPrefs.SetFloat(scoreNum, finalScore);
                PlayerPrefs.SetString(dateNum, DateTime.Now.ToString("yyyy-MM-dd"));
                return;
            }

            frontScoreNum = "score" + (i - 1);
            frontDateNum = "date" + (i - 1);    

            PlayerPrefs.SetFloat(scoreNum, PlayerPrefs.GetFloat(frontScoreNum)); // ���� score�� �ڷ� �ѱ��
            PlayerPrefs.SetString(dateNum, PlayerPrefs.GetString(frontDateNum));
        }
    }
}

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
 * GameManager 클래스 
 * 싱글톤 이용하여 다른 씬이나 다른 오브젝트에서 GameManager 클래스 속 함수나 변수 공유 가능!
 */
public class GameManager : MonoBehaviour
{
    Scene inGameScene; // 현재 인게임 씬 저장

    [SerializeField] Collider2D animalCollider;
    public bool gameOver = false;
    public bool gameClear = false;
    int callFinalScoreOnce; // 게임 끝나고 딱 한번만 기록 저장하기 위해 만든 임시변수
    // GameOver(), GameClear() 함수 각 한번씩만 호출하기 위한 변수
    //bool isGameOverCalled = false; 
    //bool isGameClearCalled = false;

    float time; // 게임 시간 기록
    float finalScore; // 최종 시간 기록
    [SerializeField] TextMeshProUGUI textTime;  // 시간을 나타내는 텍스트
    [SerializeField] GameObject textGameOver; // 게임오버 텍스트, setActive() 호출 위해서 GameObject로 
    [SerializeField] GameObject textGameClear; // 게임 클리어 텍스트

    [SerializeField] GameObject animal;
    [SerializeField] Rigidbody2D frameRigidBody;
    public GameObject wheel;
    public GameObject frame;
    private UnicycleController unicycleController;
    private PlayerDragMovement playerDragMovement;

    public static GameManager Instance { get; private set; } // 싱글톤 인스턴스

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지 -> 씬 재시작(게임 재시작)시에 변수들이 초기화되지 않는 문제가 발생! DontDestroyOnLoad는 잠시 끄는걸로
        }
        else
        {
            Destroy(gameObject); // 이미 GameManager가 존재하면 새로 생성된 오브젝트는 삭제
        }
    }


    void Start()
    {
        inGameScene = SceneManager.GetActiveScene(); // 현재 작동되고 있는 InGame씬을 저장함

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0; // 매 판마다 시간 0으로 초기화
        callFinalScoreOnce = 0; // 매 판마다 0으로 초기화
    }


    void Update()
    {

        time += Time.deltaTime;

        //textTime에 현재 시간을 표시
        if (!gameOver && !gameClear)
            textTime.text = FormatTime(time);

        // AnimalCollision.cs에서 isGround가 true가 되었다면 GameOver함수를 호출한다.
        // isGameOverCalled로 GameOver()함수가 한번만 실행되도록 함. 아니면 계속 호출됨. -> 잠시 폐기
        if (gameOver)
        {
            GameOver();
        }
        // isGameClearCalled로 GameClear()함수가 한번만 실행되도록 함. 아니면 계속 호출됨.
        if (gameClear)
        {
            GameClear();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    /* 초 단위의 time을 분과 초로 나누어서 표기함
     * 00:00 (분, 초)로 표기됨.
    */
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    /*
     * 게임오버시 여러가지 작용 구현한 함수
     */
    private void GameOver()
    {
        // 게임이 딱 끝났을 때 한번만 finalScore에 저장 / 딱 한번만 PlayerPrefs 기록 새롭게 쓰기
        // 이거 안하면 Update문 계속 호출되어 모든 기록들이 같아짐.
        if(callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            callFinalScoreOnce = 1;
        }

        
        Time.timeScale = 1f; // 시간 약간 느리게 -> 아님
        textGameOver.SetActive(true); // 게임오버 텍스트

        /* 동물 자전거에서 분리시키고, 이동방향으로 데굴데굴 구르게 */
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

        // 이동관련 스크립트 막아서 게임오버 이후 움직이지 못하게 함
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;


        //Debug.Log("몇번실행");
        // 지금 이게 Update문 안에 있으니까 계속 실행됨. 그래서 pp 기록이 다 같아지는거임
        //ScoreSorting(finalScore); // 기록 PlayerPrefs에 저장


        // 'R' 눌러 게임 재시작 -> 현재 씬을 다시 로드한다.
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    private void GameClear()
    {
        // 게임이 딱 끝났을 때 한번만 finalScore에 저장 / 딱 한번만 PlayerPrefs 기록 새롭게 쓰기
        // 이거 안하면 Update문 계속 호출되어 모든 기록들이 같아짐.
        if (callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            callFinalScoreOnce = 1;
        }


        Time.timeScale = 1f; // 시간 약간 느리게 -> 아님
        textGameClear.SetActive(true); // 게임 클리어 텍스트

        // 이동관련 스크립트 막아서 게임오버 이후 움직이지 못하게 함
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;


        // 바퀴 세워지고, 동물은 위로 점프?
        //animal.transform.SetParent(null); // 외발자전거 위에서 점프할거니까 분리?
        frameRigidBody.freezeRotation = true;


        Vector2 v = new Vector2(-0.1f, 0);
        if(frameRigidBody.velocity.magnitude <0.00001)
        {
            frameRigidBody.AddForce(v, ForceMode2D.Force);
            Debug.Log(frameRigidBody.velocity.magnitude);
        }
        // 만약 속도가 0이 되면 자전거 각을 90도로 세운다.
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
        // 'R' 눌러 게임 재시작 -> 현재 씬을 다시 로드한다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    /*
     * 스코어를 넣으면 PlayerPrefs에 기존 스코어들의 순서에 맞춰 새롭게 기록한다. 
     * 즉, 정렬되어 기록들이 저장.
     * 나중에 클리어 구현할 때 사용!
     */
    private void RecordNewScore(float finalScore)
    {
        int newScoreIdx = 0; // 새로운 기록이 저장될 Index
        string scoreNum; // PlayerPrefs에 접근할 string
        string frontScoreNum; // 새로운 기록 끼워넣을때 scoreNum의 앞 가리키기

        string dateNum;
        string frontDateNum;

        // 새로운 기록이 몇번째 기록인가 찾기
        for(int i=0; i<5; i++)
        {
            scoreNum = "score" + i;

            if(finalScore < PlayerPrefs.GetFloat(scoreNum))
            { 
                newScoreIdx = i;
                break;
            }
            if (i == 4) return; // 5개의 기록들에 속하지 않으면 아예 기록 X 그리고 return
        }

        // 새로운 기록 PlayerPrefs에 newScoreIdx에 끼워넣기
        // 뒤에서부터 앞으로. 뒤에서부터 한칸 앞의 기록들이 뒤로 이동
        for(int i = 4; i>=newScoreIdx; i--)
        {
            scoreNum = "score" + i;
            dateNum = "date" + i;

            if (i == newScoreIdx) // 만약 새롭게 기록될 index라면 기록하고 튀어
            {
                PlayerPrefs.SetFloat(scoreNum, finalScore);
                PlayerPrefs.SetString(dateNum, DateTime.Now.ToString("yyyy-MM-dd"));
                return;
            }

            frontScoreNum = "score" + (i - 1);
            frontDateNum = "date" + (i - 1);    

            PlayerPrefs.SetFloat(scoreNum, PlayerPrefs.GetFloat(frontScoreNum)); // 앞의 score를 뒤로 넘긴다
            PlayerPrefs.SetString(dateNum, PlayerPrefs.GetString(frontDateNum));
        }
    }
}

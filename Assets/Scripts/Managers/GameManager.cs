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
    int onlyOnce; // 게임 끝나고 딱 한번만 기록 저장하기 위해 만든 임시변수

    float time; // 게임 시간 기록
    float finalScore; // 최종 시간 기록
    [SerializeField] TextMeshProUGUI textTime;  // 시간을 나타내는 텍스트
    [SerializeField] GameObject textGameOver; // 게임오버 텍스트, setActive() 호출 위해서 GameObject로 

    [SerializeField] GameObject animal;
    [SerializeField] Rigidbody2D frameRigidBody;
    public GameObject wheel;
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

    // Start is called before the first frame update
    void Start()
    {
        inGameScene = SceneManager.GetActiveScene(); // 현재 작동되고 있는 InGame씬을 저장함

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0; // 매 판마다 시간 0으로 초기화
        onlyOnce = 0; // 매 판마다 0으로 초기화

    }

    // Update is called once per frame
    void Update()
    {

        time += Time.deltaTime;

        //textTime에 현재 시간을 표시
        if (!gameOver)
            textTime.text = FormatTime(time);

        // AnimalCollision.cs에서 isGround가 true가 되었다면 GameOver함수를 호출한다.
        if (gameOver) GameOver();
    }

    // 초 단위의 time을 분과 초로 나누어서 표기함
    // 00:00 (분, 초)로 표기됨.
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes} : {remainSeconds:00}";
    }

    // 게임오버시에 여러가지 작용 구현
    private void GameOver()
    {
        // 게임이 딱 끝났을 때 한번만 finalScore에 저장, 딱 한번만 PlayerPrefs 기록 새롭게 쓰기
        if(onlyOnce == 0)
        {
            finalScore = time;
            ScoreSorting(finalScore);

            onlyOnce = 1;
            //Debug.Log(FormatTime(finalScore));
        }

        

        // 플레이어프렙스에 43.4431s 형식으로 스코어 저장 -> 업적화면에서 관리 -> 배열 만들어서 정렬하고 5개 기록만 남기고 나머지 자른다는 형식으로 관리
        //PlayerPrefs.SetFloat("score", finalScore); 

        Time.timeScale = 1f; // 시간 약간 느리게
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



        // 게임 재시작 -> 현재 씬을 다시 로드한다.
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    /*
     * 스코어를 넣으면 PlayerPrefs에 기존 스코어들의 순서에 맞춰 새롭게 기록한다.
     * 클리어에 사용!
     */
    private void ScoreSorting(float finalScore)
    {
        int newScoreIdx = 0; // 새로운 기록이 저장될 Index
        string scoreNum; // PlayerPrefs에 접근할 string
        string frontScoreNum; // 새로운 기록 끼워넣을때 scoreNum의 앞 가리키기

        // 새로운 기록이 몇번째 기록인가 찾기
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

        // 새로운 기록 PlayerPrefs에 끼워넣기
        for(int i = 4; i>=newScoreIdx; i--)
        {
            scoreNum = "score" + i;

            if (i == newScoreIdx)
            {
                PlayerPrefs.SetFloat(scoreNum, finalScore);
                return;
            }

            frontScoreNum = "score" + (i - 1);

            PlayerPrefs.SetFloat(scoreNum,PlayerPrefs.GetFloat(frontScoreNum)); // 전의 score를 앞으로 넘긴다
        }
    }
}

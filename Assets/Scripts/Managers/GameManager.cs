using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Properties;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
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
    public bool gamePaused = false; // 게임 중단 -> 모자 썼을 때, 설정창
    int gameOverScoreOnce; // 게임오버 기록 딱 한번만 저장하기 위해 만든 임시변수
    int callFinalScoreOnce; // 게임 클리어 기록 //
    int callReplaceJointOnce;

    float time; // 게임 시간 기록
    public float finalScore; // 최종 시간 기록
    public bool isNewRecord = false; // NEW RECORD! 출력 여부 결정
    [SerializeField] TextMeshProUGUI textTime;  // 시간을 나타내는 텍스트
    [SerializeField] GameObject textGameOver; // 게임오버 텍스트, setActive() 호출 위해서 GameObject로 
    [SerializeField] GameObject textGameClear; // 게임 클리어 텍스트
    [SerializeField] GameObject playAgainBtn;
    [SerializeField] GameObject particles1, particles2;

    [SerializeField] GameObject animal;
    [SerializeField] Rigidbody2D frameRigidBody;
    [SerializeField] GameObject wheel;
    [SerializeField] GameObject frame;
    private UnicycleController unicycleController;
    private PlayerDragMovement playerDragMovement;
    int jumpTwice = 0;

    public bool newHatGenerated = false;
    // 모자 떨어트렸을 때
    [SerializeField] GameObject hat;
    SpriteRenderer hatSP;
    public float fadeDuration = 100f;
    public bool hatFall = false;
    public bool hatOn = false;
    public bool initializeHatOnce = true;

    // k->keyboard(UnicycleController.cs), t->touch(PlayerDragMovement.cs)
    // UnicycleController에 넣어도 될듯?
    public bool kJumpWithNoHat = false; // 모자가 없는 상태에서 점프해서 모자를 먹었을 때 오류를 막기 위한 변수. 자세한 설명 -> UnicycleController.cs
    public bool tJumpWithNoHat = false;
    private float timer;

    //효과음
    public AudioClip gameOverClip;
    public AudioClip gameClearClip;
    public AudioClip clearCheersClip;
    private int sfxOnce; // Update()문 안에 gameOVer,gameClear클립들이 호출되기 때문에, 이 클립들을 한번만 호출하기 위한 변수

    //어디서 죽었는지(클리어) 알아내기. AchieveGameOverScore로 넘길 recentSeason변수. 알맞은 clock 이미지 배치 위해.
    private float distance = 0;
    private int recentSeason = -1;
    private float startPos = 0;

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
        startPos = animal.transform.position.x;

        inGameScene = SceneManager.GetActiveScene(); // 현재 작동되고 있는 InGame씬을 저장함

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0; // 매 판마다 시간 0으로 초기화
        gameOverScoreOnce = 0; // 매 판마다 0으로 초기화

        sfxOnce = 0; // 다시 시작하면 0으로
    }

    void FixedUpdate()
    {
        //게임이 중단되지 않는 이상 시간점수 센다.
        if(!gamePaused && !hatOn) time += Time.deltaTime;
        //모자이벤트 -> 1.5배 느리게 저장
        if (hatOn) time += Time.deltaTime * 0.5f;

        //textTime에 현재 시간을 표시
        if (!gameOver && !gameClear)
            textTime.text = FormatTime(time);

        // AnimalCollision.cs에서 isGround가 true가 되었다면 GameOver함수를 호출한다.
        // isGameOverCalled로 GameOver()함수가 한번만 실행되도록 함. 아니면 계속 호출됨. -> 잠시 폐기
        if (gameOver)
        {
            if(sfxOnce == 0)
            {
                SoundManager.Instance.SFXPlay("GameOver",gameOverClip);
                sfxOnce = 1;
            }
            GameOver();
        }
        // isGameClearCalled로 GameClear()함수가 한번만 실행되도록 함. 아니면 계속 호출됨.
        if (gameClear)
        {
            if (sfxOnce == 0)
            {
                SoundManager.Instance.SFXPlay("GameClear", gameClearClip);
                SoundManager.Instance.SFXPlay("ClearCheers", clearCheersClip);
                sfxOnce = 1;
            }
            GameClear();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }

        //Find와 GetComponent를 한번만 호출하기 위해(오브젝트 할당) initializeHatOnce 변수를 사용하여 Upadte함수 내에서 호출
        if (hatFall && initializeHatOnce)
        {
            hat = GameObject.Find("Hat(Clone)");
            hatSP = hat.GetComponent<SpriteRenderer>(); // HatFall()에 사용되는 스프라이트 렌더러 초기화
            initializeHatOnce = false;
        }
        if (hatFall && !hat.IsDestroyed())
        {
            HatFall();
        }

        // 최대 도달 거리 업데이트
        if (animal.transform.position.x > distance)
        {
            distance = animal.transform.position.x - startPos;
            //Debug.Log(distance);
        }
    }

    /*
     * 모자 떨어졌을 때의 상호작용, 애니메이션
     */
    private void HatFall()
    {
        //모자와 캐릭터 충돌막기
        BoxCollider2D hatCollider = hat.GetComponent <BoxCollider2D>();
        CircleCollider2D wheelCollider = wheel.GetComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(hatCollider, wheelCollider); // 
        hat.layer = 3; // 더이상 캐릭터와 부딪히지 않게 레이어를 바꾼다.

        //모자가 머리에서 떨어졌을 때
        //투명도를 서서히 낮추고 해당 모자를 없앤다.
        timer += Time.deltaTime;
        Color color = hatSP.color;
        color.a = Mathf.Lerp(color.a, 0, timer / fadeDuration);
        hatSP.color = color;

        if (color.a < 0.01) // 모자 사라질시
        {
            // 다음 모자 생성되었을 때를 생각해 변수 초기화
            initializeHatOnce = true; 
            hatFall = false;

            Destroy(hat);
        }
    }

    /* 
     * 초 단위의 time을 분과 초로 나누어서 표기함
     * 00:00 (분, 초)로 표기됨.
    */
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes:00} : {remainSeconds:00}";
    }

    /*
     * 게임오버시 여러가지 작용 구현한 함수
     */
    private void GameOver()
    {
        // 게임이 딱 끝났을 때 한번만 finalScore에 저장 / 딱 한번만 PlayerPrefs 기록 새롭게 쓰기
        // 이거 안하면 Update문 계속 호출되어 모든 기록들이 같아짐.
        // 게임오버 기록은 단순 테스트 용임. 실제 출시땐 필요없음.
        /*if(callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            callFinalScoreOnce = 1;
        }*/
        
        // 게임오버 기록을 저장하는 하나의 PlayerPrefs
        if(gameOverScoreOnce == 0)
        {
            float gameOverScore = time;
            PlayerPrefs.SetFloat("recentScore", gameOverScore);
            PlayerPrefs.SetString("recentDate", DateTime.Now.ToString("yyyy-MM-dd"));

            gameOverScoreOnce = 1;

            CurrentSeason(distance);
        }

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

        
        // 'R' 눌러 게임 재시작 -> 현재 씬을 다시 로드한다.
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
        if(!playAgainBtn.activeSelf)
        {
            playAgainBtn.SetActive(true);
        }
    }

    /*
     * 게임 클리어시 여러 작용 구현한 함수
     */
    private void GameClear()
    {
        // 게임이 딱 끝났을 때 한번만 finalScore에 저장 / 딱 한번만 PlayerPrefs 기록 새롭게 쓰기
        // 이거 안하면 Update문 계속 호출되어 모든 기록들이 같아짐.
        if (callFinalScoreOnce == 0)
        {
            float recentScore = time;
            PlayerPrefs.SetFloat("recentScore", recentScore);
            PlayerPrefs.SetString("recentDate", DateTime.Now.ToString("yyyy-MM-dd"));

            finalScore = time;
            RecordNewScore(finalScore);

            textGameClear.SetActive(true); // 게임 클리어 텍스트
            // 폭죽 이미지
            particles1.SetActive(true);
            particles2.SetActive(true);

            callFinalScoreOnce = 1;

            CurrentSeason(distance);
        }

        // 이동관련 스크립트 막아서 게임오버 이후 움직이지 못하게 함
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;

        //frame이 회전하지 못하게
        frameRigidBody.freezeRotation = true;

        // 현재 각도를 오일러 각도로 변환
        float currentZRotation = frameRigidBody.transform.rotation.eulerAngles.z;
        if (currentZRotation > 180f) currentZRotation -= 360;

        // 자전거를 서서히 세운다.
        if (Mathf.Abs(currentZRotation) > 0.1)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            float rotationSpeed = 0.3f;
            if(currentZRotation < 0)
            {
                frameRigidBody.transform.rotation = Quaternion.RotateTowards(frameRigidBody.transform.rotation, targetRotation, rotationSpeed);
            }
            else
            {
                frameRigidBody.transform.rotation = Quaternion.RotateTowards(frameRigidBody.transform.rotation, targetRotation, rotationSpeed);
            }
        }
        else
        {
            // 속도가 줄어들면 animal 점프
            if(frameRigidBody.velocity.magnitude < 0.1)
            {
                // 점프 한번
                if (animal.GetComponent<Rigidbody2D>() == null)
                {
                    textGameClear.SetActive(false);
                    animal.transform.SetParent(null); // 외발자전거 위에서 점프할거니까 분리

                    animal.AddComponent<Rigidbody2D>();
                    Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();

                    animalRigidBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
                    jumpTwice++;
                }
                // 점프 두번, 세번
                else if(jumpTwice == 1)
                {
                    StartCoroutine(clearJump1());
                    StartCoroutine(clearJump2());
                    jumpTwice++;
                }
            }
        }


        // 'R' 눌러 게임 재시작 -> 현재 씬을 다시 로드한다.
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(inGameScene.name);
        }
    }

    // recentSeason에 게임오버된 혹은 게임클리어시에 계절을 저장
    private void CurrentSeason(float distance)
    {
        // 24.12.28 기준 봄 50m, 여름 50m, 가을 55m, 겨울 60m, 봄2 10m
        if (distance < 50 * 1) recentSeason = 0; // 봄
        else if (distance < 100 * 1) recentSeason = 1; // 여름
        else if (distance < 155 * 1) recentSeason = 2; // 가을
        else if (distance < 215 * 1) recentSeason = 3; // 겨울
        else if (distance < 300 * 1) recentSeason = 0; // spring2 - 임시값

        PlayerPrefs.SetInt("recentSeason", recentSeason);
    }


    // 코루틴으로 구현한 점프. 첫 번째, 두 번째 점프와 시간차를 두고 뛴다.
    IEnumerator clearJump1()
    {
        yield return new WaitForSecondsRealtime(1.1f);
        Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();
        animalRigidBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
    IEnumerator clearJump2()
    {
        yield return new WaitForSecondsRealtime(2.2f);
        Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();
        animalRigidBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
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
        for(int i=0; i<10; i++)
        {
            scoreNum = "score" + i;

            if(finalScore < PlayerPrefs.GetFloat(scoreNum))
            { 
                newScoreIdx = i;
                break;
            }
            if (i == 4) return; // 5개의 기록들에 속하지 않으면 아예 기록 X 그리고 return
        }

        if(newScoreIdx == 0) isNewRecord = true;
        // 새로운 기록 PlayerPrefs에 newScoreIdx에 끼워넣기
        // 뒤에서부터 앞으로. 뒤에서부터 한칸 앞의 기록들이 뒤로 이동
        for(int i = 9; i>=newScoreIdx; i--)
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

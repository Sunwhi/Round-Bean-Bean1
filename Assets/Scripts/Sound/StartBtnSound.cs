using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartBtnSound : MonoBehaviour
{
    public static StartBtnSound Instance { get; private set; }
    //메인화면의 start버튼을 통해서 인게임으로 들어왔을때만 효과음 나도록 하기 위해 변수 playSound 선언
    private int playSound;
    public AudioClip startBtnSound;
    public AudioSource audioSource;
    void Awake()
    {
        playSound = 0;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 넘어가도 삭제되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 메인화면 -> 인게임시에만 한번 재생
        if (SceneManager.GetActiveScene().name == "InGame" && playSound == 0)
        {
            audioSource.clip = startBtnSound;
            audioSource.Play();
            playSound = 1; // 인게임씬에서는 더 이상 조건문이 실행되지 않는다.
        }
        // 메인화면 돌아가면 다시 playSound = 0
        if (SceneManager.GetActiveScene().name == "MainScene") playSound = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * GameManager 클래스 
 * 싱글톤 이용하여 다른 씬이나 다른 오브젝트에서 GameManager 클래스 속 함수나 변수 공유 가능!
 */
public class GameManager : MonoBehaviour
{
    [SerializeField] Collider2D animalCollider;
    public bool isGround = false;
    private bool gameOver;

    public static GameManager Instance { get; private set; } // 싱글톤 인스턴스

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지
        }
        else
        {
            Destroy(gameObject); // 이미 GameManager가 존재하면 새로 생성된 오브젝트는 삭제
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // AnimalCollision.cs에서 isGround가 true가 되었다면 GameOver함수를 호출한다.
        if (isGround) GameOver();
    }

    // 게임을 멈춘다.
    private void GameOver()
    {
        Time.timeScale = 0;
        gameOver = true;
    }
}

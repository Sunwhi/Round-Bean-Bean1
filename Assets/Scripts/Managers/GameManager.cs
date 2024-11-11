using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Collider2D animalCollider;
    private bool isGround = false;
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
        if (isGround) GameOver();
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        gameOver = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GGround"))
        {
            Debug.Log("collision");
            isGround = true;
        }
    }
}

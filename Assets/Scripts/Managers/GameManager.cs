using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Collider2D animalCollider;
    private bool isGround = false;
    private bool gameOver;

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

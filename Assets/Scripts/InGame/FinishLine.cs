using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    public float finish = 100;

    void Start()
    {
        this.transform.position = new Vector2(finish, 0); // 결승선 위치 결정
    }

    void Update()
    {
        
    }

    // 플레이어가 결승선을 지날 시 게임 클리어
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // game clear script here
            Debug.Log("게임 클리어 판정");
        }
    }
}

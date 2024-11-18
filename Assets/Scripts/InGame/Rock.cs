using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public GameObject player;
    float cameraHalfWidth;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Game over
        }
    }
    private void Start()
    {
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
    }
    void Update()
    {
        if (transform.position.x < player.transform.position.x) // 조건문 문제인지 return이 아직 제대로 되지 않음. 생성은 잘 되고 있으므로 천천히 원인 찾아볼 것
        {
            ObjectPool.ReturnObject(this);
            Debug.Log("returned object");
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
            GameManager.Instance.gameOver = true;
    }
}

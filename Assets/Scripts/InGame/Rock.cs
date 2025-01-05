using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public GameObject player;
    float cameraHalfWidth;
    private void Start()
    {
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
    }
    void Update()
    {
        if (transform.position.x < player.transform.position.x - cameraHalfWidth - 19) 
            // player.transform.position.x - cameraHalfWidth - 19는 타일이 사라지는 위치. 여기서 돌도 소멸시킴
        {
            ObjectPool.ReturnObject(this);
            Debug.Log("returned object");
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 모자는 돌과 부딪혀도 게임오버되면 안되므로 예외처리함.
        if(!collision.gameObject.CompareTag("Hat"))
        {
            GameManager.Instance.gameOver = true;

        }
    }
}

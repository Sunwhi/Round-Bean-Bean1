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
        if (transform.position.x < player.transform.position.x - cameraHalfWidth - 19) // 조건문 문제인지 return이 아직 제대로 되지 않음. 생성은 잘 되고 있으므로 천천히 원인 찾아볼 것
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

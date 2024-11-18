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
        if (transform.position.x < player.transform.position.x) // ���ǹ� �������� return�� ���� ����� ���� ����. ������ �� �ǰ� �����Ƿ� õõ�� ���� ã�ƺ� ��
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

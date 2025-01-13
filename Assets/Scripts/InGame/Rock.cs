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
        if (transform.position.x < player.transform.position.x - cameraHalfWidth - 19) // ���ǹ� �������� return�� ���� ����� ���� ����. ������ �� �ǰ� �����Ƿ� õõ�� ���� ã�ƺ� ��
        {
            ObjectPool.ReturnObject(this);
            Debug.Log("returned object");
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ���ڴ� ���� �ε����� ���ӿ����Ǹ� �ȵǹǷ� ����ó����.
        if(!collision.gameObject.CompareTag("Hat"))
        {
            GameManager.Instance.gameOver = true;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField]
    public float finish = 100;

    void Start()
    {
        this.transform.position = new Vector2(finish, 0); // ��¼� ��ġ ����
    }

    void Update()
    {
        
    }

    // �÷��̾ ��¼��� ���� �� ���� Ŭ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // game clear script here
            Debug.Log("���� Ŭ���� ����");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class UnicycleController : MonoBehaviour
{
    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce = 100f;
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private bool isGround = false;
    private float angularSpeed;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // �¿� ȭ��ǥ Ű�� ���� ��� (ȸ���� ����)
        if (Input.GetKey(KeyCode.A))
        {
            frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // �ð� ���� ȸ��
        }

        // A, D Ű�� ������ ȸ������ ����/����
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime); // �������� ȸ��
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime); // ���������� ȸ��
        }
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
        }
        
        // ��ȸ�� �ӵ� ����, �ʹ� ������ �ʰ�
        /*angularSpeed = frameRigidbody.velocity.magnitude;
        if (angularSpeed > 4)
        {
            frameRigidbody.velocity -= frameRigidbody.velocity - Vector2()
        }*/
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
}

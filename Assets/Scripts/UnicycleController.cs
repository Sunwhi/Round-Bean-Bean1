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
            /*if (frameRigidbody.velocity.magnitude > 1.0f && frameRigidbody.velocity.x >= 0) // ȸ�� �ӵ� ����
                frameRigidbody.AddTorque(-balanceForce * 1f * Time.deltaTime);
            else
                frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��*/
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameRigidbody.AddTorque(-balanceForce * Time.deltaTime);
            /*if (frameRigidbody.velocity.magnitude > 1.0f && frameRigidbody.velocity.x < 0) // ȸ�� �ӵ� ����
                frameRigidbody.AddTorque(balanceForce * 1f * Time.deltaTime);
            else
                frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��*/
        }
        /*
        if (frameRigidbody.velocity.magnitude > 1.0f && frameRigidbody.velocity.x >= 0) // ȸ�� �ӵ� ����
            frameRigidbody.AddTorque(-balanceForce * 1f * Time.deltaTime);
        if (frameRigidbody.velocity.magnitude > 1.0f && frameRigidbody.velocity.x < 0) // ȸ�� �ӵ� ����
            frameRigidbody.AddTorque(balanceForce * 1f * Time.deltaTime);
        */
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
        
        //��ȸ�� �ӵ� ����, �ʹ� ������ �ʰ�
        /*angularSpeed = frameRigidbody.velocity.magnitude; // angularSpeed: ȸ�� �ӷ�
        if (angularSpeed > 4)
        {
            if(frameRigidbody.velocity.x >= 0)
            {
                frameRigidbody.velocity -= frameRigidbody.velocity - (frameRigidbody.velocity / frameRigidbody.velocity * 4);
            }
            else
            {
                frameRigidbody.velocity -= frameRigidbody.velocity + (frameRigidbody.velocity / frameRigidbody.velocity * 4);
            }
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

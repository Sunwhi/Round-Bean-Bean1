using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/*
 * Ű����� �����ϴ� ��ũ��Ʈ
 */
public class UnicycleController : MonoBehaviour
{
    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private bool isGround = false;
    [SerializeField] GameObject hat;
    
    [SerializeField] GameObject currentHatPosition; // ������ child�� �������ν� �������� �� ������ ��ġ�� ���󰡰�
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // A,D Ű�� ���� ��� (ȸ���� ����)
        if (Input.GetKey(KeyCode.A))
        {
            frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // �ð� ���� ȸ��
        }
        
        // �¿���� Ű�� ������ ȸ������ ����/����
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // �������� ������
            wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

            //���������� ���ٰ� �������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
            if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                frameRigidbody.AddTorque(700f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // ���������� ������
            wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime); 

            //�������� ���ٰ� ���������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
            if(frameRigidbody.angularVelocity > 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                frameRigidbody.AddTorque(-700f * Time.deltaTime);   
        }

        // �����̽��� ����
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
        }
    }
    private void Update()
    {
        // �����ϱ� �������� currentHatPosition�� ������ ��ġ�� ����(��������)�� ����
        if(isGround)
        {
            currentHatPosition.transform.position = hat.transform.position; 
            currentHatPosition.transform.rotation = hat.transform.rotation;
        }
        // ���ڸ� ���� �Ӹ� ���� ����(���� ��������)
        if (GameManager.Instance.hatOn && !isGround)
        {
            hat.transform.position = currentHatPosition.transform.position;
            hat.transform.rotation = currentHatPosition.transform.rotation;
        }
    }

    // �ٴڿ� ��������� ������ �� �ְ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GGround"))
        {
            isGround = true;
        }
    }
}

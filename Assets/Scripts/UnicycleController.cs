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

        // 좌우 화살표 키로 균형 잡기 (회전력 적용)
        if (Input.GetKey(KeyCode.A))
        {
            frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // 반시계 방향 회전
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // 시계 방향 회전
        }

        // A, D 키로 바퀴를 회전시켜 전진/후진
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime); // 왼쪽으로 회전
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime); // 오른쪽으로 회전
        }
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
        }
        
        // 각회전 속도 조절, 너무 빠르지 않게
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

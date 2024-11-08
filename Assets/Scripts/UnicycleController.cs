using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class UnicycleController : MonoBehaviour
{
    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private bool isGround = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // A,D 키로 균형 잡기 (회전력 적용)
        if (Input.GetKey(KeyCode.A))
        {
            frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // 반시계 방향 회전
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // 시계 방향 회전
        }
        
        // 좌우방향 키로 바퀴를 회전시켜 전진/후진
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // 왼쪽으로 굴러감
            wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

            //오른쪽으로 가다가 왼쪽으로 확 꺾을 때 관성을 줄이기 위한 조건문
            if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                frameRigidbody.AddTorque(700f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // 오른쪽으로 굴러감
            wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime); 

            //왼쪽으로 가다가 오른쪽으로 확 꺾을 때 관성을 줄이기 위한 조건문
            if(frameRigidbody.angularVelocity > 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                frameRigidbody.AddTorque(-700f * Time.deltaTime);   
        }

        // 스페이스바 점프
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
        }
    }
    
    // 바닥에 닿았을때만 점프할 수 있게
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GGround"))
        {
            isGround = true;
        }
    }
}

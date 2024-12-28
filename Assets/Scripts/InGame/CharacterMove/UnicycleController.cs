using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/*
 * 키보드로 조작하는 스크립트
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
    
    [SerializeField] GameObject currentHatPosition; // 동물의 child로 놓음으로써 점프했을 때 동물의 위치를 따라가게
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
    private void Update()
    {
        // 점프하기 직전까지 currentHatPosition에 모자의 위치를 로컬(동물기준)로 저장
        if(isGround)
        {
            currentHatPosition.transform.position = hat.transform.position; 
            currentHatPosition.transform.rotation = hat.transform.rotation;
        }
        // 모자를 동물 머리 위에 부착(동물 기준으로)
        if (GameManager.Instance.hatOn && !isGround)
        {
            hat.transform.position = currentHatPosition.transform.position;
            hat.transform.rotation = currentHatPosition.transform.rotation;
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

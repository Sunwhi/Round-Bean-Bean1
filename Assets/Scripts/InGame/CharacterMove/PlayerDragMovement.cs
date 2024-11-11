using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 화면 드래그를 통해 플레이어 움직임을 구현한다.
 */
public class PlayerDragMovement : MonoBehaviour
{
    private Vector2 initialTouchPosition;
    private bool isTouched;
    private float screenHalfWidth = Screen.width / 2;

    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGround = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 터치를 시작했을 때
            if(touch.phase == TouchPhase.Began)
            {

                initialTouchPosition = touch.position; // 처음 터치 위치 저장
                isTouched = true; // 터치중임
            }
            // 터치한 상태로 움직일 때(드래그)
            else if((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
            {

                Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
                Vector2 dragDirection = currentTouchPosition - initialTouchPosition; // 드래그방향

                //화면 왼쪽 터치
                if (initialTouchPosition.x < screenHalfWidth)
                {
                    if (dragDirection.x < 0)
                    {
                        frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // 반시계 방향 회전
                    }
                    else if (dragDirection.x > 0)
                    {
                        frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // 시계 방향 회전
                    }
                }
                //화면 오른쪽 터치
                else if (initialTouchPosition.x > screenHalfWidth)
                {
                    if (dragDirection.x < 0)
                    {
                        // 왼쪽으로 굴러감
                        wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

                        if(dragDirection.y < 0 && isGround)
                        {
                            if(touch.phase == TouchPhase.Ended)
                            {
                                wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                                isGround = false;
                            }
                        }

                        //오른쪽으로 가다가 왼쪽으로 확 꺾을 때 관성을 줄이기 위한 조건문
                        if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                            frameRigidbody.AddTorque(700f * Time.deltaTime);
                    }
                    else if (dragDirection.x > 0)
                    {
                        // 오른쪽으로 굴러감
                        wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime);

                        if (dragDirection.y < 0 && isGround)
                        {
                            if (touch.phase == TouchPhase.Ended)
                            {
                                wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                                isGround = false;
                            }
                        }

                        //왼쪽으로 가다가 오른쪽으로 확 꺾을 때 관성을 줄이기 위한 조건문
                        if (frameRigidbody.angularVelocity > 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                            frameRigidbody.AddTorque(-700f * Time.deltaTime);
                    }
                }
            }
            // 점프!
            else if(touch.phase == TouchPhase.Ended)
            {
                Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
                Vector2 dragDirection = currentTouchPosition - initialTouchPosition; // 드래그방향

                if(dragDirection.y<0 && isGround && (initialTouchPosition.x > screenHalfWidth))
                {
                    wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    isGround = false;
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GGround"))
        {
            isGround = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/*
 * 화면 드래그를 통해 플레이어 움직임을 구현한다.
 */
public class PlayerDragMovement : MonoBehaviour
{
    // 왼쪽 오른쪽 터치 각각 따로 보관하는 딕셔너리
    private Dictionary<int, Vector2> initialTouchPositions = new Dictionary<int, Vector2>();
    private bool isTouched;
    private float screenHalfWidth;

    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isGround = false;
    [SerializeField] GameObject hat;
    [SerializeField] GameObject currentHatPosition;

    [SerializeField] GameObject textGoForward; // 역행 금지 안내 텍스트.
    // groundscroller에서 함수를 가져오려 했으나 키보드 기반 조작 코드를 삭제하게 될 경우를 고려해 분리 작성하였음.

    // Start is called before the first frame update
    void Start()
    {
        screenHalfWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //for문 안 쓰면 터치 개수가 한개일 때 문제 발생
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                TouchControl(touch);
            }
        }
        // 점프하기 직전까지 currentHatPosition에 모자의 위치를 로컬(동물기준)로 저장
        if (isGround)
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
    private void TouchControl(Touch touch)
    {
        // 터치에 고유한 fingerId 할당
        int touchId = touch.fingerId;

        if (touch.phase == TouchPhase.Began)
        {
            if(!initialTouchPositions.ContainsKey(touchId))
            {
                initialTouchPositions[touchId] = touch.position;
            }
            isTouched = true; // 터치중임
        }

        if(initialTouchPositions.ContainsKey(touchId))
        {
            Vector2 initialTouchPosition = initialTouchPositions[touchId];

            if (initialTouchPosition.x < screenHalfWidth)
            {
                LeftTouchControl(touch, initialTouchPosition);
            }
            else
            {
                RightTouchControl(touch, initialTouchPosition);
            }
        }

        if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            initialTouchPositions.Remove(touchId);
        }
    }

    private void LeftTouchControl(Touch touch, Vector2 leftInitialTouchPosition)
    {
        //Debug.Log(touch.fingerId);
        // 터치한 상태로 움직일 때(드래그)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
            Vector2 dragDirection = currentTouchPosition - leftInitialTouchPosition; // 드래그방향

            //Debug.Log("화면 왼쪽");
            if (dragDirection.x < 0)
            {
                frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // 반시계 방향 회전
            }
            else
            {
                frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // 시계 방향 회전
            }
        }
    }
    private void RightTouchControl(Touch touch, Vector2 rightInitialTouchPosition)
    {
        //Debug.Log(touch.fingerId);
        // 터치한 상태로 움직일 때(드래그)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
            Vector2 dragDirection = currentTouchPosition - rightInitialTouchPosition; // 드래그방향

            //Debug.Log("화면 오른쪽");
            if (dragDirection.x < 0)
            {
                // 왼쪽으로 굴러감
                wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

                if (dragDirection.y < 0 && isGround)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        isGround = false;
                    }
                }

                //오른쪽으로 가다가 왼쪽으로 확 꺾을 때 관성을 줄이기 위한 조건문
                if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                    frameRigidbody.AddTorque(700f * Time.deltaTime);
            }
            else
            {
                // 일시정지 상황이 아닌데 게임이 멈춰 있는 경우, 즉 역행 방지로 인한 정지 상황인 경우
                if (Time.timeScale == 0 && !GameManager.Instance.gamePaused)
                {
                    Time.timeScale = 1; // 일시정지 해제
                    textGoForward.SetActive(false); // 안내 텍스트 숨김
                }
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
        // 점프!
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
            Vector2 dragDirection = currentTouchPosition - rightInitialTouchPosition; // 드래그방향

            if (dragDirection.y < 0 && isGround && (rightInitialTouchPosition.x > screenHalfWidth))
            {
                wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGround = false;
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

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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
    [SerializeField] GameObject backgroundPanel;

    Vector3 currentRotation;

    [SerializeField] GameObject textGoForward; // 역행 금지 안내 텍스트.
    // groundscroller에서 함수를 가져오려 했으나 키보드 기반 조작 코드를 삭제하게 될 경우를 고려해 분리 작성하였음.

    //효과음들
    public AudioClip jumpClip;
    // Start is called before the first frame update
    void Start()
    {
        screenHalfWidth = Screen.width / 2;
        //Instantiate(hat); // 모자 테스트 위해
    }
    private bool initializeHatOnce = true;

    void Update()
    {
        //for문 안 쓰면 터치 개수가 한 개일 때 문제 발생
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                //backgroundPanel이 활성화되어있을때, 옵션창이나 업적창이 열려있으면 뒤에 화면 터치 되지 않도록
                if(!touch.IsUnityNull() && !backgroundPanel.activeSelf) TouchControl(touch); 
            }
        }

        // *모자 파트
        // 점프한 상태에서 모자 먹었을 시 정상적으로 머리 위로 착용되도록 함.
        // 모자 쓴 상태에서 점프 했을 시 머리에 붙어있게 함.
        if (GameManager.Instance.newHatGenerated) initializeHatOnce = true;

        if (GameManager.Instance.hatOn && initializeHatOnce)
        {
            hat = GameObject.Find("Hat(Clone)");
            initializeHatOnce = false;
            // 모자 먹었을 때 터치 딕셔너리 초기화.
            // 안하면 오른쪽 터치한채로(이동하면서) 모자 먹고, 오른쪽 떼고 왼쪽 터치하면 오른쪽 터치작용이 일어남.
            // -> 문제점 : 오른쪽 터치한채로 모자 먹으면 움직임이 멈춤.
            // initialTouchPositions.Clear(); //*****************************************
        }
        // 점프 한 상태에서 모자를 썼을 때 바닥의 위치로 모자가 이동하는 오류를 고치기 위해
        // 모자가 있이 점프한 상태에서만 모자가 머리에 붙어있도록 바꿈
        if (!GameManager.Instance.tJumpWithNoHat && !hat.IsDestroyed())
        {

            currentRotation = hat.transform.eulerAngles;

            // Z값을 -180 ~ 180 범위로 변환 (Unity는 0 ~ 360으로 반환하기 때문에)
            float zRotation = currentRotation.z;
            if (zRotation > 180f)
            {
                zRotation -= 360f;  // 180° 이상이면 -360°을 더해줌
            }

            // 점프하기 직전까지 currentHatPosition에 모자의 위치를 로컬(동물기준)로 저장
            if (GameManager.Instance.hatOn && isGround)
            {
                currentHatPosition.transform.position = hat.transform.position;
                currentHatPosition.transform.rotation = hat.transform.rotation;
            }
            // 점프 했을 때 모자를 동물 머리 위에 부착(동물 기준으로)
            if (GameManager.Instance.hatOn && !isGround)
            {
                // 모자가 어느정도 기울어지면 더이상 점프해도 머리에 붙어있지 않게
                if (zRotation > -50 && zRotation < 50)
                {
                    // 모자가 x축 방향으로 어느정도 떨어지면 점프해도 머리에 붙어있지 않게
                    if (currentHatPosition.transform.localPosition.x < 0.5 && currentHatPosition.transform.localPosition.x > -0.5)
                    {
                        hat.transform.position = currentHatPosition.transform.position;
                        hat.transform.rotation = currentHatPosition.transform.rotation;
                    }

                }

            }
        }
        if (isGround) GameManager.Instance.tJumpWithNoHat = false; // 땅에 닿아있을 때는 항상 false로
    }


    private void TouchControl(Touch touch)
    {
        // 터치에 고유한 fingerId 할당
        int touchId = touch.fingerId;
        // 터치 시작
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

    /*
     * 왼쪽 화면 터치와 관련된 작업을 처리한다.
     * 기울기 균형
     */
    private void LeftTouchControl(Touch touch, Vector2 leftInitialTouchPosition)
    {
        // 터치한 상태로 움직일 때(드래그)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
            Vector2 dragDirection = currentTouchPosition - leftInitialTouchPosition; // 드래그방향

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
    /*
     * 오른쪽 화면 터치와 관련된 작업을 처리한다.
     * 움직임, 점프
     */
    private void RightTouchControl(Touch touch, Vector2 rightInitialTouchPosition)
    {
        // 터치한 상태로 움직일 때(드래그)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // 드래그한 후 위치를 계속 새로 받는다.
            Vector2 dragDirection = currentTouchPosition - rightInitialTouchPosition; // 드래그방향

            if (dragDirection.x < 0)
            {
                // 왼쪽으로 굴러감
                wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

                // 오른쪽으로 가다가 왼쪽으로 확 꺾을 때 관성을 줄이기 위한 조건문
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
                SoundManager.Instance.SFXPlay("Jumpp", jumpClip); 

                wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGround = false;
                // 점프 한 상태, 모자가 머리에 없다 -> jumpWithNoHat:true 
                if (!GameManager.Instance.hatOn) GameManager.Instance.tJumpWithNoHat = true;
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

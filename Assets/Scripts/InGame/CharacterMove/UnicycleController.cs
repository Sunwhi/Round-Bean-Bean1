using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/*
 * 키보드로 조작하는 스크립트
 */
public class UnicycleController : MonoBehaviour
{
    public bool enableScript = true;
    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private bool isGround = false;
    [SerializeField] GameObject hat;

    Vector3 currentRotation; // 모자 z

    [SerializeField] GameObject currentHatPosition; // 동물의 child로 놓음으로써 점프했을 때 동물의 위치를 따라가게

    //효과음들
    public AudioClip jumpClip;

    // Start is called before the first frame update
    void Start()
    {
        if(!enableScript) GetComponent<UnicycleController>().enabled = false;
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
            SoundManager.Instance.SFXPlay("Jump", jumpClip);

            wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
            // 점프 한 상태, 모자가 머리에 없다 -> jumpWithNoHat:true 
            if (!GameManager.Instance.hatOn) GameManager.Instance.kJumpWithNoHat = true;
        }

        if(isGround) GameManager.Instance.kJumpWithNoHat = false; // 땅에 닿아있을 때는 항상 false로 
        // jumpWithNoHat은 모자가 없고 점프했을 때에만 true가 된다. 그리고 딱 이 순간에만 모자를 먹었을 때 예외처리를 한다.
    }

    private bool initializeHatOnce = true;
    private void Update()
    {
        
        // Update코드 전체 주석처리하면 PlayerDragMovement에서 점프시 모자 붙어있음
        if (GameManager.Instance.newHatGenerated) initializeHatOnce = true;

        if (GameManager.Instance.hatOn && initializeHatOnce)
        {
            hat = GameObject.Find("Hat(Clone)");
            initializeHatOnce = false;
        }
        // 점프 한 상태에서 모자를 썼을 때 바닥의 위치로 모자가 이동하는 오류를 고치기 위해
        // 모자가 있이 점프한 상태에서만 모자가 머리에 붙어있도록 바꿈.
        // 즉 점프한 상태에서 모자를 먹었을 때에만 아래의 함수가 실행이 안됨.
        if (!GameManager.Instance.kJumpWithNoHat && !hat.IsDestroyed())
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
                if(zRotation > -50 && zRotation < 50)
                {
                    if (currentHatPosition.transform.localPosition.x < 0.5 && currentHatPosition.transform.localPosition.x > -0.5)
                    {
                        hat.transform.position = currentHatPosition.transform.position;
                        hat.transform.rotation = currentHatPosition.transform.rotation;
                    }
                }

            }
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

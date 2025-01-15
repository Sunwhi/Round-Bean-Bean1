using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Security;

public class Hat : MonoBehaviour
{
    [SerializeField] Transform hatTargetPosition;
    [SerializeField] GameObject frame;
    [SerializeField] GameObject wheel;
    [SerializeField] Rigidbody2D frameRigidBody;
    [SerializeField] Rigidbody2D wheelRigidBody;
    private UnicycleController unicycleController;
    private PlayerDragMovement playerDragMovement;
    private Rigidbody2D rigidBody;
    private bool hatOn; // 모자를 착용하고 있는 상태
    private bool hatFall; // 모자를 착용하다 떨어진 상태
    Vector3 hatTargetRotation;

    // int one = 1; // 한번만 실행
    // Start is called before the first frame update
    void Start()
    {
        hatTargetPosition = GameObject.Find("HatPosition").transform;
        frame = GameObject.Find("Frame");
        wheel = GameObject.Find("Wheel");
        frameRigidBody = frame.GetComponent<Rigidbody2D>();
        wheelRigidBody = wheel.GetComponent<Rigidbody2D>();

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent<PlayerDragMovement>();
        rigidBody = transform.GetComponent<Rigidbody2D>();

        hatTargetRotation = hatTargetPosition.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (hatOn)
        {
            //모자를 동물 머리 위에 부착
            //this.transform.position = hatTargetPosition.position;
            //this.transform.rotation = hatTargetPosition.rotation;
            /*
            Vector3 currentRotation = frame.transform.eulerAngles;

            // Z값을 -180 ~ 180 범위로 변환 (Unity는 0 ~ 360으로 반환하기 때문에)
            float zRotation = currentRotation.z;
            if (zRotation > 180f)
            {
                zRotation -= 360f;  // 180° 이상이면 -360°을 더해줌
            }

            // Z값이 -50 ~ 50 사이일 때 조건문 실행
            if (zRotation >= -50f && zRotation <= 50f)
            {
                //Debug.Log("Z 회전 값이 -50° ~ 50° 사이에 있습니다.");
                // 여기에 원하는 로직을 추가
            }
            else
            {
                Debug.Log(zRotation);
                hatOn = false;
                hatFall = true;
                GameManager.Instance.hatFall = true;
                GameManager.Instance.hatOn = false;
            }
            */
            //모자가 타겟 포지션보다 y축으로 아래로 1만큼 떨어지면 hatFall판정
            //HatFall!
            if (hatTargetPosition.position.y - transform.position.y > 1)
            {
                hatOn = false;
                hatFall = true;
                GameManager.Instance.hatFall = true;
                GameManager.Instance.hatOn = false;
                GameManager.Instance.newHatGenerated = false;
            }
        }  
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(!hatFall) HatAnimation();
        }
        /*if (collision.gameObject.CompareTag("GGround") && hatOn)
        {
            hatOn = false;
            hatFall = true;
            GameManager.Instance.hatFall = true;
            GameManager.Instance.hatOn = false;
        }*/
    }
    void HatAnimation()
    {
        GameManager.Instance.gamePaused = true;
        // 조작 불가
        unicycleController.enabled = false;
        playerDragMovement.enabled = false;
        frameRigidBody.freezeRotation = true;
        // 자전거 움직임 멈춤
        frameRigidBody.velocity = Vector3.zero;
        wheelRigidBody.velocity = Vector3.zero;
        frameRigidBody.isKinematic = true;
        wheelRigidBody.isKinematic = true;
        // 모자의 rigidbody 물리작용 멈춤
        //rigidBody.simulated = false;

        hatTargetRotation = hatTargetPosition.rotation.eulerAngles;
        // 모자 애니메이션
        transform.DORotate(hatTargetRotation, 1f)
            .SetEase(Ease.InBounce);
        transform.DOMove(hatTargetPosition.position, 1.0f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => // 애니메이션이 끝났으면(모자가 원하는 위치에 도달했으면)
            {
                unicycleController.enabled = true;
                playerDragMovement.enabled = true;

                frameRigidBody.freezeRotation = false;

                frameRigidBody.isKinematic = false;
                wheelRigidBody.isKinematic = false;

                //rigidBody.simulated = true;
                hatOn = true;
                GameManager.Instance.hatOn = true;
                GameManager.Instance.gamePaused = false;
            });
    }

}
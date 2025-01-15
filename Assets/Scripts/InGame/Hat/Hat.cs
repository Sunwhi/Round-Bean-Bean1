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
    private bool hatOn; // ���ڸ� �����ϰ� �ִ� ����
    private bool hatFall; // ���ڸ� �����ϴ� ������ ����
    Vector3 hatTargetRotation;

    // int one = 1; // �ѹ��� ����
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
            //���ڸ� ���� �Ӹ� ���� ����
            //this.transform.position = hatTargetPosition.position;
            //this.transform.rotation = hatTargetPosition.rotation;
            /*
            Vector3 currentRotation = frame.transform.eulerAngles;

            // Z���� -180 ~ 180 ������ ��ȯ (Unity�� 0 ~ 360���� ��ȯ�ϱ� ������)
            float zRotation = currentRotation.z;
            if (zRotation > 180f)
            {
                zRotation -= 360f;  // 180�� �̻��̸� -360���� ������
            }

            // Z���� -50 ~ 50 ������ �� ���ǹ� ����
            if (zRotation >= -50f && zRotation <= 50f)
            {
                //Debug.Log("Z ȸ�� ���� -50�� ~ 50�� ���̿� �ֽ��ϴ�.");
                // ���⿡ ���ϴ� ������ �߰�
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
            //���ڰ� Ÿ�� �����Ǻ��� y������ �Ʒ��� 1��ŭ �������� hatFall����
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
        // ���� �Ұ�
        unicycleController.enabled = false;
        playerDragMovement.enabled = false;
        frameRigidBody.freezeRotation = true;
        // ������ ������ ����
        frameRigidBody.velocity = Vector3.zero;
        wheelRigidBody.velocity = Vector3.zero;
        frameRigidBody.isKinematic = true;
        wheelRigidBody.isKinematic = true;
        // ������ rigidbody �����ۿ� ����
        //rigidBody.simulated = false;

        hatTargetRotation = hatTargetPosition.rotation.eulerAngles;
        // ���� �ִϸ��̼�
        transform.DORotate(hatTargetRotation, 1f)
            .SetEase(Ease.InBounce);
        transform.DOMove(hatTargetPosition.position, 1.0f)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => // �ִϸ��̼��� ��������(���ڰ� ���ϴ� ��ġ�� ����������)
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
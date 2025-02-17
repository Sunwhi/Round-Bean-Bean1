using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
/*
 * ȭ�� �巡�׸� ���� �÷��̾� �������� �����Ѵ�.
 */
public class PlayerDragMovement : MonoBehaviour
{
    // ���� ������ ��ġ ���� ���� �����ϴ� ��ųʸ�
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

    [SerializeField] GameObject textGoForward; // ���� ���� �ȳ� �ؽ�Ʈ.
    // groundscroller���� �Լ��� �������� ������ Ű���� ��� ���� �ڵ带 �����ϰ� �� ��츦 ����� �и� �ۼ��Ͽ���.

    //ȿ������
    public AudioClip jumpClip;
    // Start is called before the first frame update
    void Start()
    {
        screenHalfWidth = Screen.width / 2;
        //Instantiate(hat); // ���� �׽�Ʈ ����
    }
    private bool initializeHatOnce = true;
    // Update is called once per frame
    void Update()
    {
        //for�� �� ���� ��ġ ������ �� ���� �� ���� �߻�
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                //backgroundPanel�� Ȱ��ȭ�Ǿ�������, �ɼ�â�̳� ����â�� ���������� �ڿ� ȭ�� ��ġ ���� �ʵ���
                if(!touch.IsUnityNull() && !backgroundPanel.activeSelf) TouchControl(touch); 
            }
        }

        // ���� ��Ʈ
        // ������ ���¿��� ���� �Ծ��� �� ���������� �Ӹ� ���� ����ǵ��� ��.
        // ���� �� ���¿��� ���� ���� �� �Ӹ��� �پ��ְ� ��.
        if (GameManager.Instance.newHatGenerated) initializeHatOnce = true;

        if (GameManager.Instance.hatOn && initializeHatOnce)
        {
            hat = GameObject.Find("Hat(Clone)");
            initializeHatOnce = false;
            // ���� �Ծ��� �� ��ġ ��ųʸ� �ʱ�ȭ.
            // ���ϸ� ������ ��ġ��ä��(�̵��ϸ鼭) ���� �԰�, ������ ���� ���� ��ġ�ϸ� ������ ��ġ�ۿ��� �Ͼ.
            // -> ������ : ������ ��ġ��ä�� ���� ������ �������� ����.
            // initialTouchPositions.Clear(); //*****************************************
        }
        // ���� �� ���¿��� ���ڸ� ���� �� �ٴ��� ��ġ�� ���ڰ� �̵��ϴ� ������ ��ġ�� ����
        // ���ڰ� ���� ������ ���¿����� ���ڰ� �Ӹ��� �پ��ֵ��� �ٲ�
        if (!GameManager.Instance.tJumpWithNoHat && !hat.IsDestroyed())
        {

            currentRotation = hat.transform.eulerAngles;

            // Z���� -180 ~ 180 ������ ��ȯ (Unity�� 0 ~ 360���� ��ȯ�ϱ� ������)
            float zRotation = currentRotation.z;
            if (zRotation > 180f)
            {
                zRotation -= 360f;  // 180�� �̻��̸� -360���� ������
            }

            // �����ϱ� �������� currentHatPosition�� ������ ��ġ�� ����(��������)�� ����
            if (GameManager.Instance.hatOn && isGround)
            {
                currentHatPosition.transform.position = hat.transform.position;
                currentHatPosition.transform.rotation = hat.transform.rotation;
            }
            // ���� ���� �� ���ڸ� ���� �Ӹ� ���� ����(���� ��������)
            if (GameManager.Instance.hatOn && !isGround)
            {
                // ���ڰ� ������� �������� ���̻� �����ص� �Ӹ��� �پ����� �ʰ�
                if (zRotation > -50 && zRotation < 50)
                {
                    // ���ڰ� x�� �������� ������� �������� �����ص� �Ӹ��� �پ����� �ʰ�
                    if (currentHatPosition.transform.localPosition.x < 0.5 && currentHatPosition.transform.localPosition.x > -0.5)
                    {
                        hat.transform.position = currentHatPosition.transform.position;
                        hat.transform.rotation = currentHatPosition.transform.rotation;
                    }

                }

            }
        }
        if (isGround) GameManager.Instance.tJumpWithNoHat = false; // ���� ������� ���� �׻� false��
    }


    private void TouchControl(Touch touch)
    {
        // ��ġ�� ������ fingerId �Ҵ�
        int touchId = touch.fingerId;
        if (touch.phase == TouchPhase.Began)
        {
            if(!initialTouchPositions.ContainsKey(touchId))
            {
                initialTouchPositions[touchId] = touch.position;
            }
            isTouched = true; // ��ġ����
        }
        if(initialTouchPositions.ContainsKey(touchId)) // �̰� �ȵ�.
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
            
        if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) // ����x
        {
            initialTouchPositions.Remove(touchId);
        }
    }

    private void LeftTouchControl(Touch touch, Vector2 leftInitialTouchPosition)
    {
        // ��ġ�� ���·� ������ ��(�巡��)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
            Vector2 dragDirection = currentTouchPosition - leftInitialTouchPosition; // �巡�׹���

            if (dragDirection.x < 0)
            {
                frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��
            }
            else
            {
                frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // �ð� ���� ȸ��
            }
        }
    }
    private void RightTouchControl(Touch touch, Vector2 rightInitialTouchPosition)
    {
        // ��ġ�� ���·� ������ ��(�巡��)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
            Vector2 dragDirection = currentTouchPosition - rightInitialTouchPosition; // �巡�׹���

            if (dragDirection.x < 0)
            {
                // �������� ������
                wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

                // �Ʒ��� ���ȴ� ���� ����
                /*if (dragDirection.y < 0 && isGround)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        isGround = false;
                        if (!GameManager.Instance.hatOn) GameManager.Instance.tJumpWithNoHat = true;

                    }
                }*/

                // ���������� ���ٰ� �������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
                if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                    frameRigidbody.AddTorque(700f * Time.deltaTime);
            }
            else
            {
                // �Ͻ����� ��Ȳ�� �ƴѵ� ������ ���� �ִ� ���, �� ���� ������ ���� ���� ��Ȳ�� ���
                if (Time.timeScale == 0 && !GameManager.Instance.gamePaused)
                {
                    Time.timeScale = 1; // �Ͻ����� ����
                    textGoForward.SetActive(false); // �ȳ� �ؽ�Ʈ ����
                }

                // ���������� ������
                wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime);

                // �Ʒ��� ���ȴ� ���� ����
                /*if (dragDirection.y < 0 && isGround)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        isGround = false;
                        if (!GameManager.Instance.hatOn) GameManager.Instance.tJumpWithNoHat = true;

                    }
                }*/

                //�������� ���ٰ� ���������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
                if (frameRigidbody.angularVelocity > 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                    frameRigidbody.AddTorque(-700f * Time.deltaTime);
            }
        }
        // ����!
        else if (touch.phase == TouchPhase.Ended)
        {
            Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
            Vector2 dragDirection = currentTouchPosition - rightInitialTouchPosition; // �巡�׹���

            if (dragDirection.y < 0 && isGround && (rightInitialTouchPosition.x > screenHalfWidth))
            {
                SoundManager.Instance.SFXPlay("Jumpp", jumpClip); 

                wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGround = false;
                // ���� �� ����, ���ڰ� �Ӹ��� ���� -> jumpWithNoHat:true 
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

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    [SerializeField] GameObject textGoForward; // ���� ���� �ȳ� �ؽ�Ʈ.
    // groundscroller���� �Լ��� �������� ������ Ű���� ��� ���� �ڵ带 �����ϰ� �� ��츦 ����� �и� �ۼ��Ͽ���.

    // Start is called before the first frame update
    void Start()
    {
        screenHalfWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //for�� �� ���� ��ġ ������ �Ѱ��� �� ���� �߻�
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                TouchControl(touch);
            }
        }
        // �����ϱ� �������� currentHatPosition�� ������ ��ġ�� ����(��������)�� ����
        if (isGround)
        {
            currentHatPosition.transform.position = hat.transform.position;
            currentHatPosition.transform.rotation = hat.transform.rotation;
        }
        // ���ڸ� ���� �Ӹ� ���� ����(���� ��������)
        if (GameManager.Instance.hatOn && !isGround)
        {
            hat.transform.position = currentHatPosition.transform.position;
            hat.transform.rotation = currentHatPosition.transform.rotation;
        }
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
        // ��ġ�� ���·� ������ ��(�巡��)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
            Vector2 dragDirection = currentTouchPosition - leftInitialTouchPosition; // �巡�׹���

            //Debug.Log("ȭ�� ����");
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
        //Debug.Log(touch.fingerId);
        // ��ġ�� ���·� ������ ��(�巡��)
        if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
        {
            Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
            Vector2 dragDirection = currentTouchPosition - rightInitialTouchPosition; // �巡�׹���

            //Debug.Log("ȭ�� ������");
            if (dragDirection.x < 0)
            {
                // �������� ������
                wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

                if (dragDirection.y < 0 && isGround)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        isGround = false;
                    }
                }

                //���������� ���ٰ� �������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
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

                if (dragDirection.y < 0 && isGround)
                {
                    if (touch.phase == TouchPhase.Ended)
                    {
                        wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        isGround = false;
                    }
                }

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

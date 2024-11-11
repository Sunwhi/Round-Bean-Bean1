using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ȭ�� �巡�׸� ���� �÷��̾� �������� �����Ѵ�.
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

            // ��ġ�� �������� ��
            if(touch.phase == TouchPhase.Began)
            {

                initialTouchPosition = touch.position; // ó�� ��ġ ��ġ ����
                isTouched = true; // ��ġ����
            }
            // ��ġ�� ���·� ������ ��(�巡��)
            else if((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isTouched)
            {

                Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
                Vector2 dragDirection = currentTouchPosition - initialTouchPosition; // �巡�׹���

                //ȭ�� ���� ��ġ
                if (initialTouchPosition.x < screenHalfWidth)
                {
                    if (dragDirection.x < 0)
                    {
                        frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��
                    }
                    else if (dragDirection.x > 0)
                    {
                        frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // �ð� ���� ȸ��
                    }
                }
                //ȭ�� ������ ��ġ
                else if (initialTouchPosition.x > screenHalfWidth)
                {
                    if (dragDirection.x < 0)
                    {
                        // �������� ������
                        wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

                        if(dragDirection.y < 0 && isGround)
                        {
                            if(touch.phase == TouchPhase.Ended)
                            {
                                wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                                isGround = false;
                            }
                        }

                        //���������� ���ٰ� �������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
                        if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                            frameRigidbody.AddTorque(700f * Time.deltaTime);
                    }
                    else if (dragDirection.x > 0)
                    {
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
            }
            // ����!
            else if(touch.phase == TouchPhase.Ended)
            {
                Vector2 currentTouchPosition = touch.position; // �巡���� �� ��ġ�� ��� ���� �޴´�.
                Vector2 dragDirection = currentTouchPosition - initialTouchPosition; // �巡�׹���

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

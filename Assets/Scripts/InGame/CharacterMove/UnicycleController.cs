using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/*
 * Ű����� �����ϴ� ��ũ��Ʈ
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

    Vector3 currentRotation; // ���� z

    [SerializeField] GameObject currentHatPosition; // ������ child�� �������ν� �������� �� ������ ��ġ�� ���󰡰�

    //ȿ������
    public AudioClip jumpClip;

    // Start is called before the first frame update
    void Start()
    {
        if(!enableScript) GetComponent<UnicycleController>().enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // A,D Ű�� ���� ��� (ȸ���� ����)
        if (Input.GetKey(KeyCode.A))
        {
            frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // �ݽð� ���� ȸ��
        }
        else if (Input.GetKey(KeyCode.D))
        {
            frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // �ð� ���� ȸ��
        }
        
        // �¿���� Ű�� ������ ȸ������ ����/����
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // �������� ������
            wheelRigidbody.AddTorque(moveSpeed * Time.deltaTime);

            //���������� ���ٰ� �������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
            if (frameRigidbody.angularVelocity < 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                frameRigidbody.AddTorque(700f * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            // ���������� ������
            wheelRigidbody.AddTorque(-moveSpeed * Time.deltaTime); 

            //�������� ���ٰ� ���������� Ȯ ���� �� ������ ���̱� ���� ���ǹ�
            if(frameRigidbody.angularVelocity > 0 && Mathf.Abs(frameRigidbody.angularVelocity) > 150f)
                frameRigidbody.AddTorque(-700f * Time.deltaTime);   
        }

        // �����̽��� ����
        if (Input.GetKey(KeyCode.Space) && isGround)
        {
            SoundManager.Instance.SFXPlay("Jump", jumpClip);

            wheelRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
            // ���� �� ����, ���ڰ� �Ӹ��� ���� -> jumpWithNoHat:true 
            if (!GameManager.Instance.hatOn) GameManager.Instance.kJumpWithNoHat = true;
        }

        if(isGround) GameManager.Instance.kJumpWithNoHat = false; // ���� ������� ���� �׻� false�� 
        // jumpWithNoHat�� ���ڰ� ���� �������� ������ true�� �ȴ�. �׸��� �� �� �������� ���ڸ� �Ծ��� �� ����ó���� �Ѵ�.
    }

    private bool initializeHatOnce = true;
    private void Update()
    {
        
        // Update�ڵ� ��ü �ּ�ó���ϸ� PlayerDragMovement���� ������ ���� �پ�����
        if (GameManager.Instance.newHatGenerated) initializeHatOnce = true;

        if (GameManager.Instance.hatOn && initializeHatOnce)
        {
            hat = GameObject.Find("Hat(Clone)");
            initializeHatOnce = false;
        }
        // ���� �� ���¿��� ���ڸ� ���� �� �ٴ��� ��ġ�� ���ڰ� �̵��ϴ� ������ ��ġ�� ����
        // ���ڰ� ���� ������ ���¿����� ���ڰ� �Ӹ��� �پ��ֵ��� �ٲ�.
        // �� ������ ���¿��� ���ڸ� �Ծ��� ������ �Ʒ��� �Լ��� ������ �ȵ�.
        if (!GameManager.Instance.kJumpWithNoHat && !hat.IsDestroyed())
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

    // �ٴڿ� ��������� ������ �� �ְ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("GGround"))
        {
            isGround = true;
        }
    }
}

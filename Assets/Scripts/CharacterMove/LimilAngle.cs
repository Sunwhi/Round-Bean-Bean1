using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleComtrol : MonoBehaviour
{
    [SerializeField] Rigidbody2D frameRigidbody;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float zRotation = transform.rotation.eulerAngles.z;
        // �¿�� 90���� ���� �ʵ��� ���ӵ��� ����
        if (zRotation > 90f && zRotation < 270f) // 90������ 270�� ���̸� �Ѿ�ٰ� �Ǵ�
        {
            frameRigidbody.angularVelocity = 0f; // ȸ���� ���ߵ��� ���ӵ��� 0���� ����

            // ȸ�� ������ 90�� �Ǵ� 270���� �����Ͽ� �� ���� ���� ���� ����
            if (zRotation > 180f) // 180 ~ 270��
            {
                frameRigidbody.rotation = 270f; // �������� 90�� ����
            }
            else // 90 ~ 180��
            {
                frameRigidbody.rotation = 90f; // ���������� 90�� ����
            }
        }
    }
}

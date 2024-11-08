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
        // 좌우로 90도를 넘지 않도록 각속도를 제한
        if (zRotation > 90f && zRotation < 270f) // 90도에서 270도 사이면 넘어갔다고 판단
        {
            frameRigidbody.angularVelocity = 0f; // 회전을 멈추도록 각속도를 0으로 설정

            // 회전 각도를 90도 또는 270도로 고정하여 한 바퀴 도는 것을 방지
            if (zRotation > 180f) // 180 ~ 270도
            {
                frameRigidbody.rotation = 270f; // 왼쪽으로 90도 제한
            }
            else // 90 ~ 180도
            {
                frameRigidbody.rotation = 90f; // 오른쪽으로 90도 제한
            }
        }
    }
}

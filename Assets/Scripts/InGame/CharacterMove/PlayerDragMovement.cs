using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDragMovement : MonoBehaviour
{
    private Vector2 initialTouchPosition;
    private bool isTouched;
    private float screenHalfWidth;

    [SerializeField] Rigidbody2D frameRigidbody;
    [SerializeField] Rigidbody2D wheelRigidbody;
    [SerializeField] private float balanceForce = 100f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10.0f;
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

            if(touch.phase == TouchPhase.Began)
            {
                initialTouchPosition = touch.position;
                isTouched = true;
            }
            else if(touch.phase == TouchPhase.Moved && isTouched)
            {
                Vector2 currentTouchPosition = touch.position;
                Vector2 dragDirection = currentTouchPosition - initialTouchPosition;
                screenHalfWidth = Screen.width / 2;

                if(initialTouchPosition.x < screenHalfWidth)
                {
                    if (dragDirection.x < 0)
                    {
                        frameRigidbody.AddTorque(balanceForce * Time.deltaTime); // 반시계 방향 회전
                    }
                    else if (dragDirection.x > 0)
                    {
                        frameRigidbody.AddTorque(-balanceForce * Time.deltaTime); // 시계 방향 회전
                    }
                }
            }
        }
    }
}

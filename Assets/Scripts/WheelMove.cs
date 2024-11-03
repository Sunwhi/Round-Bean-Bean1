using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float rTorque = -5.0f;
    [SerializeField] private float lTorque = 5.0f;
    private float velocityMagnitude;
    [SerializeField] private float jumpForce = 10.0f;
    Rigidbody2D rb;
    [SerializeField] private bool isGround = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocityMagnitude = rb.velocity.magnitude;

        if (Input.GetKey(KeyCode.D))
        {
            if(velocityMagnitude <= 8.0) // 최대 속도 제한
            {
                /*if(rb.velocity.x <= 0) // 왼쪽으로 이동중
                    rb.AddTorque(rTorque * 2, ForceMode2D.Force);
                else
                    rb.AddTorque(rTorque, ForceMode2D.Force);*/
                rb.AddTorque(rTorque, ForceMode2D.Force);
            }
        }
        if(Input.GetKey(KeyCode.A)) 
        {
            if(velocityMagnitude <= 8.0)
            {
                /*if (rb.velocity.x > 0) // 오른쪽으로 이동중
                    rb.AddTorque(lTorque * 2, ForceMode2D.Force);
                else
                    rb.AddTorque(lTorque, ForceMode2D.Force);*/
                rb.AddTorque(lTorque, ForceMode2D.Force);
            }           
        }
        if(Input.GetKey(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGround = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }
}

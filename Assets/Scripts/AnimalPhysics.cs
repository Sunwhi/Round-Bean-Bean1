using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimalPhysics : MonoBehaviour
{
    [SerializeField] private float magnitude = 5.0f;
    Rigidbody2D rb;
    //GameObject saddle = GameObject.Find("Saddle");
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // <기울이기 구현> 1,-1 / -1,-1 방향으로 힘을 가한다.
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector2 direction = transform.right + (transform.up * (-1f));
            direction.Normalize();

            rb.AddForce(direction * magnitude, ForceMode2D.Force);
         }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Vector2 direction = (transform.right * (-1f)) + (transform.up * (-1f));    
            direction.Normalize();

            rb.AddForce(direction * magnitude, ForceMode2D.Force);
        }
    }

}

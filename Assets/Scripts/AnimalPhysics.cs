using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AnimalPhysics : MonoBehaviour
{
    [SerializeField] private float magnitude = 5.0f;
    Rigidbody2D rb;
    //private Vector2 direction;
    //private Vector2 velocity;
    //private Vector2 acceleration;
    //private Vector2 force;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
    /*private void rightForce()
    {
        //Vector2.right
        direction = transform.right + (transform.up * (-1f));
        force = direction * magnitude;

        acceleration = force / rb.mass;
        velocity += acceleration / Time.deltaTime;

        transform.position += (Vector3)velocity * Time.deltaTime;
    }*/

}

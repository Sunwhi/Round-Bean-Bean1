using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPhysics : MonoBehaviour
{
    [SerializeField] private float force = 5.0f;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.L)) 
        {
            rb.AddForce(Vector2.right * force, ForceMode2D.Force);
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            rb.AddForce(Vector2.left * force, ForceMode2D.Force);

        }
    }
}

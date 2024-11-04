/**
 * In-game scene
 * temporarily written script for controlling map scroll as player moves.
 * Need to be replaced later.
 * basic movement(no tilting) and jump, random character among 3
 */


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool isJump = false;
    bool isGround = true;

    public float jumpPower = 0;
    public float moveSpeed = 0;
    public Sprite[] playerChar;
    SpriteRenderer spriteRenderer;
    private Rigidbody2D rigid2D;
    public GameObject ground;

    Vector2 startPosition;
    
    void Start()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        startPosition = rigid2D.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // random character
        spriteRenderer.sprite = playerChar[Random.Range(0, playerChar.Length)];
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isJump && isGround)
        {
            isGround = false;
            isJump = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // moving ground instead of player
            ground.transform.position += new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            ground.transform.position += new Vector3(-1, 0, 0) * moveSpeed * Time.deltaTime;
        }
    }
    private void FixedUpdate()
    {
        if (isJump)
        {
            isJump = false;
            rigid2D.AddForce(Vector2.up * jumpPower);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // prevent double-jump
        if (collision.gameObject.tag == "ground")
        {
            isGround = true;
        }
    }
}

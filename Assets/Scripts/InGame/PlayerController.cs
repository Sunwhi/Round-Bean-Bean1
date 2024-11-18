/**
 * In-game scene
 * 맵 스크롤링 구현을 위해 임시로 작성된 캐릭터 동작 코드.
 * 현재 다른 스크립트로 대체되었음.
 * line 34~35를 캐릭터 랜덤 시작 기능에 이용할 것.
 *   24.11.17 기준 캐릭터 이미지가 없어 미구현인 상태
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

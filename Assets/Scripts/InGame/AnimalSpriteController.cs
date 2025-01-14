using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpriteController : MonoBehaviour
{
    public Sprite[] idlePlayerChar; // 기본 상태
    public Sprite[] eventPlayerChar; // 모자, 게임오버, 클리어
    SpriteRenderer spriteRenderer;
    int playerChar;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerChar = Random.Range(0, idlePlayerChar.Length);
        // random character
        spriteRenderer.sprite = idlePlayerChar[playerChar];
    }

    // Inspector에서 eventPlayerChar의 sprite 순서는 반드시 idlePlayerChar에 넣은 캐릭터 순서대로 [A_hat, A_cry, A_smile, B_hat, ...] 과 같이 배치할 것
    void Update()
    {
        if (GameManager.Instance.hatOn) // hat on - hat
        {
            spriteRenderer.sprite = eventPlayerChar[playerChar * 3];
        }
        if (!GameManager.Instance.hatOn) // no hat - idle
        {
            spriteRenderer.sprite = idlePlayerChar[playerChar];
        }
        if (GameManager.Instance.gameOver) // game over - cry
        {
            spriteRenderer.sprite = eventPlayerChar[playerChar * 3 + 1];
        }
        if (GameManager.Instance.gameClear) // game clear - smile
        {
            spriteRenderer.sprite = eventPlayerChar[playerChar * 3 + 2];
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpriteController : MonoBehaviour
{
    public Sprite[] idlePlayerChar; // �⺻ ����
    public Sprite[] eventPlayerChar; // ����, ���ӿ���, Ŭ����
    SpriteRenderer spriteRenderer;
    int playerChar;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerChar = Random.Range(0, idlePlayerChar.Length);
        // random character
        spriteRenderer.sprite = idlePlayerChar[playerChar];
    }

    // Inspector���� eventPlayerChar�� sprite ������ �ݵ�� idlePlayerChar�� ���� ĳ���� ������� [A_hat, A_cry, A_smile, B_hat, ...] �� ���� ��ġ�� ��
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    public SpriteRenderer[] bgTiles; // ������Ʈ
    public Sprite[] bgImg; // ��� �̹�����
    float fadeDuration = 1.5f; // ��� ��ȯ �ӵ�
    public Transform player; // ��ũ�� ����: ĳ����
    private float bgWidth; // ��� �̹��� ���� ����
    [SerializeField] private float parallaxEffectMultiplier; // ��� �̵� ����
    private float lastPlayerPosition; // ĳ���� �̵� ������ ���� ����

    void Start()
    {
        bgWidth = bgTiles[0].sprite.bounds.size.x;
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        if (player == null)
        {
            player = Camera.main.transform;
        }
        lastPlayerPosition = player.position.x;
        GroundScroller.OnSeasonChanged += UpdateBG; // �̺�Ʈ ����
    }

    private void OnDisable()
    {
        GroundScroller.OnSeasonChanged -= UpdateBG; // �̺�Ʈ ���� ���� - �ʿ��Ѱ�?
    }

    void Update()
    {
        float deltax = player.position.x - lastPlayerPosition;
        transform.position += new Vector3(deltax * parallaxEffectMultiplier, 0, 0);
        lastPlayerPosition = player.position.x;

        for (int i = 0; i < bgTiles.Length; i++)
        {
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 16) // ��� �̹����� ȭ���� �����. 16�� ������ ����� �߰� ����
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // ������ ������ �̵�
            }
        }

    }

    private void UpdateBG(int season)
    {
        for (int i = 0; i < bgTiles.Length; i++)
        {
            bgTiles[i].sprite = bgImg[season];
        }
        
        // TODO: FadeBGImage�� �ϼ��ؼ� �ε巯�� ȭ�� ��ȯ ȿ�� ���� ��. 
        // StartCoroutine(FadeBGImage(season));
    }

    /// <summary>
    /// �ε巯�� ȭ�� ��ȯ ȿ��. �̿ϼ� �Լ��Դϴ�.
    /// </summary>
    /// <param name="nextSeason"></param>
    /// <returns></returns>
    private IEnumerator FadeBGImage(int nextSeason)
    {
        float elapsedTime = 0f;

        while(elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < bgTiles.Length; i++)
            {
                Color color = bgTiles[i].color;
                color.a = (i == nextSeason) ? alpha : 1f - alpha; // ���� ��� FadeIn, ���� ��� FadeOut
                bgTiles[i].color = color;
            }
            yield return null; // ���� ������ ���
        }

        // ���İ� ������ ����
        for (int i = 0; i < bgTiles.Length; i++)
        {
            Color color = bgTiles[i].color;
            color.a = (i == nextSeason) ? 1f : 0f;
            bgTiles[i].color = color;
        }
    }
}

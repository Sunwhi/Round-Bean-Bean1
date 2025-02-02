using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    [SerializeField] SpriteRenderer[] bgTiles; // ������Ʈ
    [SerializeField] SpriteRenderer[] nextBgTiles; // ���� ���� ��ȯ��
    [SerializeField] public Sprite[] bgImg; // ��� �̹�����
    [SerializeField] float fadeDuration = 1f; // ��� ��ȯ �ð�. fade out�� in ������ �ɸ��� �ð��̹Ƿ� ���� �ҿ�ð��� x2
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
        //foreach (var tile in nextBgTiles)
        //{
        //    SetAlpha(tile, 0f);
        //}
        lastPlayerPosition = player.position.x;
        GroundScroller.OnSeasonChanged += UpdateBG; // ���� ��ȭ ���� �̺�Ʈ ����
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
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 20) // ��� �̹����� ȭ���� ����� (20�� ������ ����� �߰� ����)
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // ������ ������ �̵�
                nextBgTiles[i].transform.position = bgTiles[i].transform.position; // ���� ������ Ÿ�ϵ� �Ȱ��� �̵�
            }
        }

    }

    private void UpdateBG(int season)
    { 
        StartCoroutine(FadeBGImage(season));
    }

    /// <summary>
    /// �ε巯�� ȭ�� ��ȯ ȿ��. ���� ��� fade out -> ���� ��� �̹����� ���� �� fade in
    /// </summary>
    /// <param name="nextSeason"></param>
    /// <returns></returns>
    private IEnumerator FadeBGImage(int nextSeason)
    {
        float elapsedTime = 0f;

        foreach (var tile in nextBgTiles) // ���� ������ ����� �̸� ����
        {
            tile.sprite = bgImg[nextSeason];
            SetAlpha(tile, 0f); // ���� �Ⱥ��̰�
        }

        // ��ȯ
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < bgTiles.Length; i++)
            {
                // SetAlpha(bgTiles[i], 1 - alpha); // ���� ��� Fade out
                SetAlpha(nextBgTiles[i], alpha); // ���� ��� Fade in
            }
            yield return null; // ���� ������ ���
        }

        for (int i = 0; i < bgTiles.Length; i++)
        {
            // ���İ� ������ ����
            SetAlpha(bgTiles[i], 0);
            SetAlpha(nextBgTiles[i], 1);

            // �迭 swap
            SpriteRenderer temp = bgTiles[i];
            bgTiles[i] = nextBgTiles[i];
            nextBgTiles[i] = temp;
        }
    }

    /// <summary>
    /// �̹��� ���İ� ����
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="alpha"></param>
    private void SetAlpha(SpriteRenderer renderer, float alpha)
    {
        Color color = renderer.color;
        color.a = alpha;
        renderer.color = color;
    }
}

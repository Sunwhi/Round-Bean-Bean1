using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    [SerializeField] SpriteRenderer[] bgTiles; // ������Ʈ
    [SerializeField] SpriteRenderer[] nextBgTiles; // ���� ���� ��ȯ��
    [SerializeField] public Sprite[] bgImgSet; // ��� �̹�����
    [SerializeField] float fadeDuration = 1f; // ��� ��ȯ �ð�. fade out�� in ������ �ɸ��� �ð��̹Ƿ� ���� �ҿ�ð��� x2
    public Transform player; // ��ũ�� ����: ĳ����
    private float bgWidth; // ��� �̹��� ���� ����
    [SerializeField] private float parallaxEffectMultiplier; // ��� �̵� ����. �������� ��� ��ũ�� �ӵ� ����
    private float lastPlayerPosition; // ĳ���� �̵� ������ ���� ����

    private int bgStart;
    private int bgEnd;
    private int currentImageIndex = 0;
    private int currentSeason = 0;
    [SerializeField] int springBgIndex; // bgImgSet �� �� ������ ����� ���� �ε���
    [SerializeField] int summerBgIndex;
    [SerializeField] int autumnBgIndex;
    [SerializeField] int winterBgIndex;
    [SerializeField] int spring2BgIndex;

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
        bgStart = springBgIndex;
        bgEnd = summerBgIndex;
        currentImageIndex = 2;
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
        transform.position += new Vector3(deltax * parallaxEffectMultiplier, 0, 0); // BackGround ��ü ��ü�� ����
        lastPlayerPosition = player.position.x;

        for (int i = 0; i < bgTiles.Length; i++)
        {
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 20) // ��� �̹����� ȭ���� ����� (20�� ������ ����� �߰� ����)
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // ������ ������ �̵�

                currentImageIndex = (currentImageIndex + 1) % (bgEnd - bgStart);
                bgTiles[i].sprite = bgImgSet[currentImageIndex + bgStart];
            }
        }

    }

    private void UpdateBG(int season)
    {
        currentImageIndex = 2;
        StartCoroutine(FadeBGImage(season));
    }

    /// <summary>
    /// �ε巯�� ȭ�� ��ȯ ȿ��. ���� ��� �̹����� ���� ��� ���� fade in
    /// </summary>
    /// <param name="nextSeason"></param>
    /// <returns></returns>
    private IEnumerator FadeBGImage(int nextSeason)
    {
        Debug.Log("start fading to " + nextSeason);
        float elapsedTime = 0f;
        SetSeasonBgSet(nextSeason);

        // TODO: ��ũ�Ѹ����� ���� �ٲ� Ÿ�� ������ �̹������� �״�� ��Ÿ��. ��� Ÿ���� �迭 ������� �������ϱ�

        for (int i = 0; i < nextBgTiles.Length; i++) // ���� ������ ����� �̸� ����
        {
            nextBgTiles[i].sprite = bgImgSet[(i % (bgEnd - bgStart)) + bgStart];
            SetAlpha(nextBgTiles[i], 0f); // ���� �Ⱥ��̰�
            Vector3 newPos = nextBgTiles[i].transform.position;
            newPos = new Vector3(
                player.transform.position.x + bgWidth * i - cameraHalfWidth - 4, 
                bgTiles[i].transform.position.y, 
                bgTiles[i].transform.position.z - 0.1f
                );  // �������� �ణ �տ� ��ġ
            nextBgTiles[i].transform.position = newPos;
        }

        // ��ȯ
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < bgTiles.Length; i++)
            {
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

    private void SetSeasonBgSet(int season) // change bg image range 
    {
        switch (season)
        {
            case 0:
                bgStart = springBgIndex;
                bgEnd = summerBgIndex;
                currentSeason = 0;
                parallaxEffectMultiplier = 0.3f;
                break;
            case 1:
                bgStart = summerBgIndex;
                bgEnd = autumnBgIndex;
                currentSeason = 1;
                parallaxEffectMultiplier = 0.1f;
                break;
            case 2:
                bgStart = autumnBgIndex;
                bgEnd = winterBgIndex;
                currentSeason = 2;
                parallaxEffectMultiplier = 0.1f;
                break;
            case 3:
                bgStart = winterBgIndex;
                bgEnd = spring2BgIndex;
                currentSeason = 3;
                parallaxEffectMultiplier = 0.3f;
                break;
            case 4:
                bgStart = spring2BgIndex;
                bgEnd = bgImgSet.Length;
                currentSeason = 4;
                parallaxEffectMultiplier = 0.3f;
                break;
            default:
                throw new Exception("Season index not valid");
        }
        Debug.Log("season BG set to " + season + ", currentSeason: " + currentSeason);
    }
}

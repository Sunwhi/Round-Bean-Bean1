using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    public SpriteRenderer[] bgTiles; // ������Ʈ
    public Sprite[] bgImg; // ��� �̹�����
    float fadeDuration = 0.75f; // ��� ��ȯ �ð�. fade out�� in ������ �ɸ��� �ð��̹Ƿ� ���� �ҿ�ð��� x2
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
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 20) // ��� �̹����� ȭ���� �����. 20�� ������ ����� �߰� ����
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // ������ ������ �̵�
            }
        }

    }

    private void UpdateBG(int season)
    {
        //for (int i = 0; i < bgTiles.Length; i++)
        //{
        //    bgTiles[i].sprite = bgImg[season];
        //}
        
        // TODO: FadeBGImage�� �ϼ��ؼ� �ε巯�� ȭ�� ��ȯ ȿ�� ���� ��. 
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

        #region fadeout
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            foreach (var tile in bgTiles)
            {
                Color color = tile.color;
                color.a = 1f - alpha;
                tile.color = color;
            }
            yield return null; // ���� ������ ���
        }
        #endregion

        // ������ fade out �� ����
        elapsedTime = 0f; // fade in�� ���� �ʱ�ȭ
        foreach (var tile in bgTiles)
        {
            tile.sprite = bgImg[nextSeason]; // ���� ���� �̹����� ���� (���� fade in �ϱ� ��)
        }

        #region fadein
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            foreach (var tile in bgTiles)
            {
                Color color = tile.color;
                color.a = alpha;
                tile.color = color;
            }
            yield return null; // ���� ������ ���
        }
        #endregion

        // ���İ� ������ ����
        for (int i = 0; i < bgTiles.Length; i++)
        {
            Color color = bgTiles[i].color;
            color.a = 1f;
            bgTiles[i].color = color;
        }
    }
}

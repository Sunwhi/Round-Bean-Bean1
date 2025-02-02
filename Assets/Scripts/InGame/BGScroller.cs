using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    [SerializeField] SpriteRenderer[] bgTiles; // 오브젝트
    [SerializeField] SpriteRenderer[] nextBgTiles; // 다음 계절 전환용
    [SerializeField] public Sprite[] bgImg; // 배경 이미지셋
    [SerializeField] float fadeDuration = 1f; // 배경 전환 시간. fade out과 in 각각에 걸리는 시간이므로 실제 소요시간은 x2
    public Transform player; // 스크롤 기준: 캐릭터
    private float bgWidth; // 배경 이미지 가로 길이
    [SerializeField] private float parallaxEffectMultiplier; // 배경 이동 배율
    private float lastPlayerPosition; // 캐릭터 이동 감지를 위한 변수

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
        GroundScroller.OnSeasonChanged += UpdateBG; // 계절 변화 감지 이벤트 구독
    }

    private void OnDisable()
    {
        GroundScroller.OnSeasonChanged -= UpdateBG; // 이벤트 구독 해제 - 필요한가?
    }

    void Update()
    {
        float deltax = player.position.x - lastPlayerPosition;
        transform.position += new Vector3(deltax * parallaxEffectMultiplier, 0, 0);
        lastPlayerPosition = player.position.x;

        for (int i = 0; i < bgTiles.Length; i++)
        {
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 20) // 배경 이미지가 화면을 벗어나면 (20은 역행을 대비한 추가 여유)
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // 오른쪽 끝으로 이동
                nextBgTiles[i].transform.position = bgTiles[i].transform.position; // 다음 계절용 타일도 똑같이 이동
            }
        }

    }

    private void UpdateBG(int season)
    { 
        StartCoroutine(FadeBGImage(season));
    }

    /// <summary>
    /// 부드러운 화면 전환 효과. 기존 배경 fade out -> 다음 배경 이미지로 변경 후 fade in
    /// </summary>
    /// <param name="nextSeason"></param>
    /// <returns></returns>
    private IEnumerator FadeBGImage(int nextSeason)
    {
        float elapsedTime = 0f;

        foreach (var tile in nextBgTiles) // 다음 계절의 배경을 미리 설정
        {
            tile.sprite = bgImg[nextSeason];
            SetAlpha(tile, 0f); // 아직 안보이게
        }

        // 전환
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < bgTiles.Length; i++)
            {
                // SetAlpha(bgTiles[i], 1 - alpha); // 기존 배경 Fade out
                SetAlpha(nextBgTiles[i], alpha); // 다음 배경 Fade in
            }
            yield return null; // 다음 프레임 대기
        }

        for (int i = 0; i < bgTiles.Length; i++)
        {
            // 알파값 마무리 보정
            SetAlpha(bgTiles[i], 0);
            SetAlpha(nextBgTiles[i], 1);

            // 배열 swap
            SpriteRenderer temp = bgTiles[i];
            bgTiles[i] = nextBgTiles[i];
            nextBgTiles[i] = temp;
        }
    }

    /// <summary>
    /// 이미지 알파값 조정
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

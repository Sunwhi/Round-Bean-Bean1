using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    [SerializeField] SpriteRenderer[] bgTiles; // 오브젝트
    [SerializeField] SpriteRenderer[] nextBgTiles; // 다음 계절 전환용
    [SerializeField] public Sprite[] bgImgSet; // 배경 이미지셋
    [SerializeField] float fadeDuration = 1f; // 배경 전환 시간. fade out과 in 각각에 걸리는 시간이므로 실제 소요시간은 x2
    public Transform player; // 스크롤 기준: 캐릭터
    private float bgWidth; // 배경 이미지 가로 길이
    [SerializeField] private float parallaxEffectMultiplier; // 배경 이동 배율. 낮을수록 배경 스크롤 속도 증가
    private float lastPlayerPosition; // 캐릭터 이동 감지를 위한 변수

    private int bgStart;
    private int bgEnd;
    private int currentImageIndex = 0;
    private int currentSeason = 0;
    [SerializeField] int springBgIndex; // bgImgSet 내 각 계절별 배경의 시작 인덱스
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
        GroundScroller.OnSeasonChanged += UpdateBG; // 계절 변화 감지 이벤트 구독
    }

    private void OnDisable()
    {
        GroundScroller.OnSeasonChanged -= UpdateBG; // 이벤트 구독 해제 - 필요한가?
    }

    void Update()
    {
        float deltax = player.position.x - lastPlayerPosition;
        transform.position += new Vector3(deltax * parallaxEffectMultiplier, 0, 0); // BackGround 객체 전체에 적용
        lastPlayerPosition = player.position.x;

        for (int i = 0; i < bgTiles.Length; i++)
        {
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 20) // 배경 이미지가 화면을 벗어나면 (20은 역행을 대비한 추가 여유)
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // 오른쪽 끝으로 이동

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
    /// 부드러운 화면 전환 효과. 다음 배경 이미지를 기존 배경 위에 fade in
    /// </summary>
    /// <param name="nextSeason"></param>
    /// <returns></returns>
    private IEnumerator FadeBGImage(int nextSeason)
    {
        Debug.Log("start fading to " + nextSeason);
        float elapsedTime = 0f;
        SetSeasonBgSet(nextSeason);

        // TODO: 스크롤링으로 인해 바뀐 타일 순서가 이미지에도 그대로 나타남. 배경 타일을 배열 순서대로 재정렬하기

        for (int i = 0; i < nextBgTiles.Length; i++) // 다음 계절의 배경을 미리 설정
        {
            nextBgTiles[i].sprite = bgImgSet[(i % (bgEnd - bgStart)) + bgStart];
            SetAlpha(nextBgTiles[i], 0f); // 아직 안보이게
            Vector3 newPos = nextBgTiles[i].transform.position;
            newPos = new Vector3(
                player.transform.position.x + bgWidth * i - cameraHalfWidth - 4, 
                bgTiles[i].transform.position.y, 
                bgTiles[i].transform.position.z - 0.1f
                );  // 기존보다 약간 앞에 위치
            nextBgTiles[i].transform.position = newPos;
        }

        // 전환
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < bgTiles.Length; i++)
            {
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

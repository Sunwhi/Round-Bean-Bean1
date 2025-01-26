using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroller : MonoBehaviour
{
    float cameraHalfWidth;
    public SpriteRenderer[] bgTiles; // 오브젝트
    public Sprite[] bgImg; // 배경 이미지셋
    float fadeDuration = 1.5f; // 배경 전환 속도
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
        lastPlayerPosition = player.position.x;
        GroundScroller.OnSeasonChanged += UpdateBG; // 이벤트 구독
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
            if (bgTiles[i].transform.position.x < Camera.main.transform.position.x - cameraHalfWidth - 16) // 배경 이미지가 화면을 벗어나면. 16은 역행을 대비한 추가 여유
            {
                bgTiles[i].transform.position += new Vector3(bgWidth * 3, 0, 0); // 오른쪽 끝으로 이동
            }
        }

    }

    private void UpdateBG(int season)
    {
        for (int i = 0; i < bgTiles.Length; i++)
        {
            bgTiles[i].sprite = bgImg[season];
        }
        
        // TODO: FadeBGImage를 완성해서 부드러운 화면 전환 효과 넣을 것. 
        // StartCoroutine(FadeBGImage(season));
    }

    /// <summary>
    /// 부드러운 화면 전환 효과. 미완성 함수입니다.
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
                color.a = (i == nextSeason) ? alpha : 1f - alpha; // 다음 배경 FadeIn, 현재 배경 FadeOut
                bgTiles[i].color = color;
            }
            yield return null; // 다음 프레임 대기
        }

        // 알파값 마무리 보정
        for (int i = 0; i < bgTiles.Length; i++)
        {
            Color color = bgTiles[i].color;
            color.a = (i == nextSeason) ? 1f : 0f;
            bgTiles[i].color = color;
        }
    }
}

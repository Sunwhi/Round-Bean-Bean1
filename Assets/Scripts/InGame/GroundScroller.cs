/**
 * In-game Scene
 * infinite map scrolling & obstacle spawning
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public SpriteRenderer[] tiles;
    public Sprite[] groundImg;
    public GameObject rock;
    public GameObject player;
    float cameraHalfWidth;
    SpriteRenderer temp; // for scrolling tiles

    private float startPos;
    public float distance;

    private int seasonNow = 0; // 계절 기록용 변수, 장애물 생성 가능여부 판정에 사용함. 0: spring, 1: summer, 2: autumn, 3: winter, 4: spring2(미사용)
    private int springTilesIndex = 0; // 나중에 계절당 타일 여러 개 생길 거 대비.. 지만 좋은 방법은 아닐지도. 더 나은 방식 고민해볼것
    private int summerTilesIndex = 1;
    private int autumnTilesIndex = 2;
    private int winterTilesIndex = 3;

    private int tilesStart = 0;
    private int tilesEnd = 1;

    private int obstacleCount_rock = 0;
    private int obstacleCount_cliff = 0;
    private bool rockJudge = false;
    private bool cliffJudge = false;
    private int obstacleDelay = 0;
    private float obstacleChance = 0;
    private bool[] obstacleFlag = new bool[21]; // false: judgeable, true: already judged. set to false when declared

    [SerializeField] GameObject textGoForward; // 역행 금지 안내 텍스트

    void Start()
    {
        temp = tiles[0];
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        startPos = player.transform.position.x;

        //StartCoroutine(DistanceLog());
    }
    // 1초마다 이동거리 로그에 찍기 (디버깅용)
    //private IEnumerator DistanceLog()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1);
    //        Debug.Log("distance: " + distance);
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        // 최대 도달 거리 업데이트
        if (player.transform.position.x > distance)
        {
            distance = player.transform.position.x - startPos;
        }

        // 최대 도달 거리보다 한참 뒤에 있을 때 ( = 역행 시도 시)
        if (player.transform.position.x < distance - 16 && !GameManager.Instance.gameOver) // 역행하다가 게임오버 시 텍스트가 겹치는 문제 방지
        {
            Time.timeScale = 0; // 게임 일시정지. GameManager에 함수를 추가해서 이용하는 게 더 바람직할까?
            textGoForward.SetActive(true); // 안내 텍스트 표시
        }
        if (Time.timeScale == 0 && Input.GetKey(KeyCode.RightArrow))
        {
            Time.timeScale = 1; // 일시정지 해제
            textGoForward.SetActive(false); // 안내 텍스트 숨김
        }

        // summer: 10m, autumn: 21m, winter: 33m, spring2: 45m
        if (distance < 10 * 2) SetSeason("spring");
        else if (distance < 21 * 2) SetSeason("summer");
        else if (distance < 33 * 10) SetSeason("autumn"); // 가을 이후의 장애물 디버깅을 위해 거리를 늘리는 부분 (*2를 늘리면 됨)
        else if (distance < 45 * 10) SetSeason("winter");
        else if (distance < 50 * 10) SetSeason("spring"); // spring2

        // check every tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            // set when to move the leftmost block that player already passed
            if (player.transform.position.x - cameraHalfWidth - 19 >= tiles[i].transform.position.x) // cameraHalfWidth - 11 -> cameraHalfWidth - 7
            {
                for (int q = 0; q < tiles.Length; q++)
                {
                    // find the location of the rightmost block
                    if (temp.transform.position.x < tiles[q].transform.position.x)
                    {
                        temp = tiles[q];
                    }
                }
                // move block next to the rightmost block
                tiles[i].transform.position = new Vector2(temp.transform.position.x + 2, -2f);
                SetTile(tiles[i], true); // show tiles, just in case disabled last time
                obstacleFlag[i] = false; // reloaded, judge again

                // if ground img has variations, change img randomly
                tiles[i].sprite = groundImg[UnityEngine.Random.Range(tilesStart, tilesEnd)];
            }

            if (tiles[i].sprite == groundImg[0]) obstacleFlag[i] = true; // 봄인 경우 판정 불필요, 여름 진입하자마자 바로 역행 시 봄에서 판정을 시도하는 문제 방지

            // spawning obstacles. is seperation to another file needed? and possible?
            // summer: 0.2/4m, autumn: 0.25/2m, winter: 0.3/1m
            if (player.transform.position.x + cameraHalfWidth * 0.59 <= tiles[i].transform.position.x
                && player.transform.position.x + cameraHalfWidth * 0.61 >= tiles[i].transform.position.x // when the block reaches at 80% of the display
                && !obstacleFlag[i] && seasonNow >= 1) // and spawning obstacles is not judged yet, and season is after spring 
            {
                obstacleFlag[i] = true;
                // 가을 이후 생성 알고리즘
                if (seasonNow >= 2) 
                {
                #region afterAutumn
                    // 24.11.18 생성 로직 테스트를 위해 작성된 가을 이후 절벽 로직. 돌과 동시 생성인 경우 50% 확률로 하나만 골라 생성.
                    if (obstacleCount_cliff > 0)
                    {
                        cliffJudge = ProbabilityRandom(obstacleChance);
                        rockJudge = ProbabilityRandom(obstacleChance);
                        Debug.Log("...judging cliffs and rocks..." + cliffJudge + ", " + rockJudge);
                        if (cliffJudge)
                        {
                            if (rockJudge)
                            {
                                bool cliffOrRock = ProbabilityRandom(0.5f); // true: cliff, false: rock
                                if (cliffOrRock)
                                {
                                    SetTile(tiles[i], false); // hide tile = spawn cliff
                                    Debug.Log("cliff selected");
                                }
                                else
                                {
                                    var rock = ObjectPool.GetObject();
                                    rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                                    Debug.Log("rock selected");
                                }
                            }
                            else SetTile(tiles[i], false); // hide tile
                        }
                        obstacleCount_cliff = obstacleDelay + 1; // 성공하든 실패하든 8m(4칸)마다 판정하므로 딜레이 적용.
                        obstacleCount_rock = obstacleDelay + 1;
                        Debug.Log("obstacleCount_cliff and rock : " + obstacleCount_cliff + ", " + obstacleCount_rock);
                        break;
                    }
                    else
                    {
                        obstacleCount_cliff++;
                        Debug.Log("obstacleCount_cliff: " + obstacleCount_cliff);
                        break;
                    }
                #endregion
                }
                #region summer
                // 여름 - 돌만 생성됨. 
                // 24.11.18 기준 생성 로직 테스트를 위해 전 계절에 적용 중.

                if (obstacleCount_rock > 0) // 가을 이후 절벽과의 동시 생성 여부 파악을 위해 미리 돌 판정
                {
                    rockJudge = ProbabilityRandom(obstacleChance);
                    Debug.Log("...judging rocks..." + rockJudge);
                    if (rockJudge)
                    {
                        var rock = ObjectPool.GetObject();
                        rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                    }
                    obstacleCount_rock = obstacleDelay - 1;
                    Debug.Log("obstacleCount_rock: " + obstacleCount_rock);
                }
                else
                {
                    obstacleCount_rock++;
                    Debug.Log("obstacleCount_rock: " + obstacleCount_rock);
                }
                #endregion
            }
        }
    }

    /// <summary>
    /// Random function with probability.
    /// </summary>
    /// <param name="probability">probability must be float between 0 and 1</param>
    /// <returns>true or false, depending on the probability you set</returns>
    public static bool ProbabilityRandom(float probability)
    {
        return (UnityEngine.Random.value <= probability);
    }

    /// <summary>
    /// Hide or show tiles. Used for making cliffs for now.
    /// </summary>
    /// <param name="tile">tiles that you want to hide or show</param>
    /// <param name="boolean">set false to hide, true to show</param>
    private void SetTile(SpriteRenderer tile, bool boolean)
    {
        tile.gameObject.GetComponent<Collider2D>().enabled = boolean;
        tile.gameObject.SetActive(boolean);
        // Debug.Log("SetTile Executed : " + boolean);
    }

    /// <summary>
    /// function for changing groundImg Index according to the season
    /// </summary>
    /// <param name="season">"spring" | "summer" | "autumn" | "winter". other strings are not accepted.</param>
    /// <exception cref="Exception"></exception>
    private void SetSeason(string season) // change tile image range 
    {
        switch (season)
        {
            case "spring":
                tilesStart = springTilesIndex;
                tilesEnd = summerTilesIndex;
                seasonNow = 0;
                obstacleChance = 0f; // 계절별 생성 확률 변경은 여기서 각 obstacleChance 값 변경 (1 이하)
                break;
            case "summer":
                tilesStart = summerTilesIndex;
                tilesEnd = autumnTilesIndex;
                seasonNow = 1;
                obstacleDelay = -4; // 장애물이 생성된 경우 다음 4칸 동안은 등장하지 않음. 
                obstacleChance = 0.2f;
                break;
            case "autumn":
                tilesStart = autumnTilesIndex;
                tilesEnd = winterTilesIndex;
                seasonNow = 2;
                obstacleDelay = -4; // -2;
                obstacleChance = 1f;
                break;
            case "winter":
                tilesStart = winterTilesIndex;
                tilesEnd = groundImg.Length;
                seasonNow = 3;
                obstacleDelay = -4; // -1;
                obstacleChance = 0.3f;
                break;
            default:
                throw new Exception("Season name not valid");
        }
    }

}

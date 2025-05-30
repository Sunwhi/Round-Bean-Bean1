/**
 * In-game Scene
 * infinite map scrolling & obstacle spawning
 * 맵 생성 관련 전반
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] tiles;
    [SerializeField] SpriteRenderer[] nextTiles;
    [SerializeField] Sprite[] groundImg;
    [SerializeField] SpriteRenderer[] seasonSign;

    public GameObject player;
    public GameObject hat;
    float cameraHalfWidth;
    SpriteRenderer temp; // for scrolling tiles

    private float startPos;
    public float distance;
    float tempdist = 0; // 계절 타일 변화와 장애물 알고리즘 변화 사이에 유예를 두기 위해 사용되는 변수
    bool isTempDistValid = false; // 계절 타일 변화와 장애물 알고리즘 변경 사이의 시점인 경우 True
    bool isSwapping = false; // 계절 변화 도중 땅이 스크롤되는 경우 그만큼의 구멍이 생기게 됨. 따라서 변화 도중엔 스크롤을 멈추기 위함

    public static event Action<int> OnSeasonChanged; // BGScroller.cs에서의 배경 변경을 트리거하기 위한 이벤트
    private int currentSeason = 0; // 계절 기록용 변수, 장애물 생성 가능여부 판정에 사용함. 0: spring, 1: summer, 2: autumn, 3: winter
    [SerializeField] int springTilesIndex; // groundImg 내 각 계절별 타일의 시작 인덱스. ex) 타일이 하나씩인 경우 각각 0, 1, 2, 3. 
    [SerializeField] int summerTilesIndex;
    [SerializeField] int autumnTilesIndex;
    [SerializeField] int winterTilesIndex;
    [SerializeField] int spring2TilesIndex;

    private int tilesStart;
    private int tilesEnd;
    [SerializeField] float fadeDuration = 1f;

    private int obstacleCount = 0;
    private int hatCount = 0;
    private bool rockJudge = false;
    private bool cliffJudge = false;
    private int obstacleDelay = 0;
    private float obstacleChance = 0;
    private bool[] obstacleFlag = new bool[26]; // 이미 한 번 판정을 완료한 경우 true. 배열 크기는 ground 오브젝트 갯수와 같아야 함
    private bool[] isMoving = new bool[26]; // 코루틴 동시 적용 방지

    [SerializeField] GameObject textGoForward; // 역행 금지 안내 텍스트

    // obstacle animation
    [SerializeField] float obsAnimDuration = 0.6f;

    // SFX
    public AudioClip rockSpawnClip;
    public AudioClip cliffSpawnClip;
    public AudioClip hatSpawnClip;

    void Start()
    {
        temp = tiles[0];
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        startPos = player.transform.position.x;


        tilesStart = springTilesIndex;
        tilesEnd = summerTilesIndex; // 시작 계절 set, SetSeason() 사용 시 불필요한 동작이 있음
        for (int i = 0; i < tiles.Length; i++) // 게임 시작과 동시에 타일 이미지 set
            tiles[i].sprite = groundImg[UnityEngine.Random.Range(tilesStart, tilesEnd)];
    }

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

        UpdateSeason(distance);

        // 땅 타일 스크롤 & 장애물 생성
        for (int i = 0; i < tiles.Length; i++)
        {
            // set when to move the leftmost block that player already passed
            if (!isSwapping && player.transform.position.x - cameraHalfWidth - 19 >= tiles[i].transform.position.x)
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
                tiles[i].transform.position = new Vector3(temp.transform.position.x + 2, -2f, 1f);
                SetTile(tiles[i], true); // show tiles, just in case disabled last time
                obstacleFlag[i] = false; // reloaded, judge again

                // if ground img has variations, change img randomly
                tiles[i].sprite = groundImg[UnityEngine.Random.Range(tilesStart, tilesEnd)];
            }

            if (currentSeason == 0) obstacleFlag[i] = true; // 봄인 경우 판정 불필요, 여름 진입하자마자 바로 역행 시 봄에서 판정을 시도하는 문제 방지

            // spawning obstacles. is seperation to another file needed? and possible?
            if (player.transform.position.x + cameraHalfWidth * 0.59 <= tiles[i].transform.position.x
                && player.transform.position.x + cameraHalfWidth * 0.61 >= tiles[i].transform.position.x // when the block reaches at 80% of the display
                && !obstacleFlag[i] && currentSeason >= 1 && !isSwapping) // and spawning obstacles is not judged yet, and season is after spring 
            {
                obstacleFlag[i] = true; // 중복판정 방지

                if (hatCount >= 3 && !GameManager.Instance.hatOn && obstacleCount > 0)
                // 장애물이 3번 이상 나타나지 않은 경우, 또한 현재 모자를 쓰고 있지 않은 경우. 장애물이 생길 칸에 모자 생성
                // 모자를 쓴 동안에는 hatCount = 0
                {
                    bool hatJudge = ProbabilityRandom(0.8f);
                    if (hatJudge)
                    {
                        var newHat = Instantiate(hat, new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f), new Quaternion(0, 0, 0, 0));
                        GameManager.Instance.newHatGenerated = true; // 모자 새로 생성되면 newHatGenerated 참으로 설정
                        SoundManager.Instance.SFXPlay("HatGenerated", hatSpawnClip);

                        obstacleCount = obstacleDelay + 1; // 장애물 딜레이 적용
                        hatCount = 0;
                        Debug.Log("hat spawned");
                        break; // 모자 생성 시 이후 장애물 판정은 필요없음
                    }
                    hatCount = 0; // else
                    Debug.Log("hat spawn failed");
                }

                if (currentSeason >= 2) 
                {
                #region afterAutumn
                    if (obstacleCount > 0) // 24.12.25 obstacleCount_rock과 obstacleCount_cliff 통합
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
                                    isMoving[i] = true;
                                    SpawnCliff(tiles[i], i);
                                    Debug.Log("cliff selected");
                                }
                                else
                                {
                                    SpawnRock(tiles[i]);
                                    Debug.Log("rock selected");
                                }
                                hatCount = 0;
                            }
                            else
                            {
                                isMoving[i] = true;
                                SpawnCliff(tiles[i], i);
                            }
                            hatCount = 0;
                        }
                        else if (!cliffJudge && rockJudge) // cliffJudge가 false고 rockjudge가 true인 경우에 대한 추가 처리
                        {
                            SpawnRock(tiles[i]);
                            Debug.Log("rock selected");
                            hatCount = 0;
                        }
                        obstacleCount = obstacleDelay + 1; // 성공하든 실패하든 8m(4칸)마다 판정하므로 딜레이 적용.
                        if (!(cliffJudge || rockJudge || GameManager.Instance.hatOn)) hatCount++; // 장애물 생성에 실패한 경우, 모자를 안 쓰고 있으면 hat count 증가.
                        Debug.Log("hatCount: " + hatCount);
                        break;
                    }
                    else // 아직 생성될 때가 아닌 경우 (obstacleDelay에 걸리는 경우)
                    {
                        obstacleCount++;
                        break;
                    }
                #endregion
                }
                #region summer
                // 여름 - 돌만 생성됨. 
                else if (obstacleCount > 0) // 가을 이후 절벽과의 동시 생성 여부 파악을 위해 미리 돌 판정
                {
                    rockJudge = ProbabilityRandom(obstacleChance);
                    Debug.Log("...judging rocks..." + rockJudge);
                    if (rockJudge)
                    {
                        SpawnRock(tiles[i]);
                        hatCount = 0;
                    }
                    else
                    {
                        if (!GameManager.Instance.hatOn) hatCount++;
                    }
                    obstacleCount = obstacleDelay + 1;
                    Debug.Log("hatCount: " + hatCount);
                    //Debug.Log("obstacleCount: " + obstacleCount);
                }
                else
                {
                    obstacleCount++;
                    //Debug.Log("obstacleCount: " + obstacleCount);
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
    /// Hide or show tiles. Used for map scrolling
    /// </summary>
    /// <param name="tile">tiles that you want to hide or show</param>
    /// <param name="boolean">set false to hide, true to show</param>
    private void SetTile(SpriteRenderer tile, bool boolean)
    {
        tile.gameObject.SetActive(boolean);
    }

    /// <summary>
    /// Hide tiles. Used for spawning cliffs
    /// </summary>
    /// <param name="tile"></param>
    private void SpawnCliff(SpriteRenderer tile, int i)
    {
        Vector2 cliffEndPos = new Vector2(tile.transform.position.x, tile.transform.position.y - 2);
        StartCoroutine(AnimateCliff(tile, tile.transform.position, cliffEndPos, i));
        SoundManager.Instance.SFXPlay("BluffSpawned", cliffSpawnClip); // SFX
    }

    /// <summary>
    /// Spawn rock.
    /// </summary>
    /// <param name="tile"></param>
    private void SpawnRock(SpriteRenderer tile)
    {
        var rock = ObjectPool.GetObject();
        if (currentSeason == 1)
        {
            rock.spriteRenderer.sprite = ObjectPool.instance.rockImg[UnityEngine.Random.Range(0, ObjectPool.instance.rockImg.Length)];
        }
        rock.transform.position = new Vector3(tile.transform.position.x + 0.5f, -1.5f, 1.5f); // 땅 아래에 스폰
        Vector3 rockEndPos = new Vector3(tile.transform.position.x + 0.5f, -0.8f, 1.5f); // 스폰 목표 위치
        StartCoroutine(AnimateRock(rock, rock.transform.position, rockEndPos));
        SoundManager.Instance.SFXPlay("StoneSpawned", rockSpawnClip);
    }

    /// <summary>
    /// 절벽 연출용 코루틴. startPos부터 endPos까지 Lerp로 이동.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    private IEnumerator AnimateCliff(SpriteRenderer tile, Vector2 startPos, Vector2 endPos, int i)
    {
        float elapsedTime = 0;
        while(elapsedTime < obsAnimDuration)
        {
            elapsedTime += Time.deltaTime;
            float delta = Mathf.Clamp01(elapsedTime / obsAnimDuration);
            tile.transform.position = Vector2.Lerp(startPos, endPos, delta);
            yield return null;
        }
        tile.transform.position = endPos;
        tile.gameObject.SetActive(false); // 다 내려가면 없앰
        isMoving[i] = false;
    }

    /// <summary>
    /// AnimateObstacle의 Rock 오브젝트용 오버로딩.
    /// </summary>
    /// <param name="rock"></param>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <returns></returns>
    private IEnumerator AnimateRock(Rock rock, Vector3 startPos, Vector3 endPos)
    {
        float elapsedTime = 0;
        while (elapsedTime < obsAnimDuration)
        {
            elapsedTime += Time.deltaTime;
            float delta = Mathf.Clamp01(elapsedTime / obsAnimDuration);
            rock.transform.position = Vector3.Lerp(startPos, endPos, delta);
            yield return null;
        }
        rock.transform.position = endPos;
    }

    /// <summary>
    /// 계절별 타일 변경을 위한 함수. UpdateSeason에 의해 계절이 변경될 때만 호출됨.
    /// </summary>
    /// <param name="season">0: spring, 1: summer, 2: autumn, 3: winter, 4: spring2. other values are not accepted.</param>
    /// <exception cref="Exception"></exception>
    private void SetSeasonTiles(int season) // change tile image range 
    {
        Debug.Log("season tile set to " + season + ", currentSeason: " + currentSeason);
        switch (season)
        {
            // summer: 20%/4m, autumn: 25%/2m, winter: 30%/1m
            case 0:
                tilesStart = springTilesIndex;
                tilesEnd = summerTilesIndex;
                currentSeason = 0;
                break;
            case 1:
                tilesStart = summerTilesIndex;
                tilesEnd = autumnTilesIndex;
                currentSeason = 1;
                break;
            case 2:
                tilesStart = autumnTilesIndex;
                tilesEnd = winterTilesIndex;
                currentSeason = 2;
                break;
            case 3:
                tilesStart = winterTilesIndex;
                tilesEnd = spring2TilesIndex;
                currentSeason = 3;
                break;
            case 4:
                tilesStart = spring2TilesIndex;
                tilesEnd = groundImg.Length;
                currentSeason = 4;
                break;
            default:
                throw new Exception("Season index not valid");
        }
    }

    /// <summary>
    /// 계절별 장애물 조건 변경을 위한 함수. UpdateSeason에 의해 계절이 변경될 때만 호출되며, SetSeasonTiles 호출 이후에 호출됨.
    /// 계절 타일셋 변경과 장애물 알고리즘 변경 사이에 유예를 두기 위하여 분리.
    /// </summary>
    /// <param name="season">0: spring, 1: summer, 2: autumn, 3: winter. other values are not accepted.</param>
    /// <exception cref="Exception"></exception>
    private void SetSeasonObs(int season) // change tile image range 
    {
        Debug.Log("season tile set to " + season + ", currentSeason: " + currentSeason);
        switch (season)
        {
            // summer: 20%/4m, autumn: 25%/2m, winter: 30%/1m
            case 0:
                obstacleChance = 0f;
                break;
            case 1:
                obstacleDelay = -4; // 장애물이 생성된 경우 다음 4칸 동안은 등장하지 않음. 
                obstacleChance = 0.2f; // 계절별 생성 확률 변경은 여기서 각 obstacleChance 값 변경 (0 이상 1 이하)
                break;
            case 2:
                obstacleDelay = -2;
                obstacleChance = 0.25f;
                break;
            case 3:
                obstacleDelay = -2;
                obstacleChance = 0.3f;
                break;
            case 4:
                obstacleChance = 0f;
                break;
            default:
                throw new Exception("Season index not valid");
        }
    }


    /// <summary>
    /// 계절 변화를 함수로 분리. 또한 string을 이용하던 방식 대신 currentSeason 변수 하나로 통일.
    /// 매 프레임마다 거리에 따라 계절을 반복하여 set하는 대신
    /// 계절이 바뀌는 경우에만 새롭게 set하도록 함.
    /// 매 프레임마다 호출.
    /// </summary>
    /// <param name="distance">현재 게임 플레이에서 최대 도달 거리</param>
    private void UpdateSeason(float distance)
    {
        int newSeason = 0;

        // 24.12.28 기준 봄 50m, 여름 50m, 가을 55m, 겨울 60m, 봄2 10m
        if (distance < 50 * 1 - 25) newSeason = 0; // 계절 변경 후 25m 뒤 지점부터 여름 타일로 바뀌기 시작하므로 타이밍 당기기
        else if (distance < 100 * 1 - 25) newSeason = 1;
        else if (distance < 155 * 1 - 25) newSeason = 2; // 가을 이후의 장애물 디버깅을 위해 거리를 늘리는 부분 (곱하는 수를 조정하면 됨)
        else if (distance < 215 * 1 - 25) newSeason = 3;
        else newSeason = 4; // spring2

        if (!isTempDistValid && newSeason != currentSeason)
        {
            CreateSeasonSign(newSeason, distance); // 계절 변경 안내 표지판 생성
            tempdist = distance;
            isTempDistValid = true;
        }
        if (isTempDistValid && distance > tempdist + 18) // 변경된 타일들로 실제로 넘어갈 때 장애물 알고리즘 변경. 게임 관점에선 실질적으로 계절이 바뀌는 부분.
        {
            SetSeasonObs(newSeason);
            OnSeasonChanged?.Invoke(newSeason); // UpdateBG() 트리거
            isSwapping = true;
            StartCoroutine(FadeTileImage(newSeason));
            currentSeason = newSeason;
            isTempDistValid = false;
        }
    }

    /// <summary>
    /// 계절 변경 안내 표지판 생성 함수.
    /// 계절이 바뀌는 위치에서 알아서 생성되도록 함.
    /// </summary>
    /// <param name="season">계절에 해당하는 표지판을 세우기 위함</param>
    private void CreateSeasonSign(int season, float distance)
    {
        Debug.Log("executing CreateSeasonSign with" + season + ", " + distance);
        seasonSign[season-1].transform.position = new Vector3(distance+21, 1.5f, 0.5f); // z 값이 클수록 뒤로. groundImg는 1f, 나머지는 0f이다.
        seasonSign[season-1].gameObject.SetActive(true);
    }

    private IEnumerator FadeTileImage(int nextseason)
    {
        float elapsedTime = 0f;
        SetSeasonTiles(nextseason);

        for (int i = 0; i < nextTiles.Length; i++)
        {
            nextTiles[i].sprite = groundImg[(i % (tilesEnd - tilesStart)) + tilesStart];
            SetAlpha(nextTiles[i], 0f);
            if (tiles[i].gameObject.activeSelf && !isMoving[i]) nextTiles[i].gameObject.SetActive(true);
            Vector3 newPos = nextTiles[i].transform.position;
            newPos = new Vector3(
                tiles[i].transform.position.x,
                tiles[i].transform.position.y,
                tiles[i].transform.position.z - 0.1f // 기존보다 약간 앞에 위치
                );
            nextTiles[i].transform.position = newPos;
        }
        // 전환
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            for (int i = 0; i < tiles.Length; i++)
            {
                SetAlpha(nextTiles[i], alpha); // 다음 배경 Fade in
            }
            yield return null; // 다음 프레임 대기
        }

        for (int i = 0; i < tiles.Length; i++)
        {
            // 알파값 마무리 보정
            SetAlpha(nextTiles[i], 1);

            SetAlpha(tiles[i], 0);
            tiles[i].gameObject.SetActive(false);

            // 배열 swap
            SpriteRenderer temp = tiles[i];
            tiles[i] = nextTiles[i];
            nextTiles[i] = temp;
        }

        isSwapping = false;
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

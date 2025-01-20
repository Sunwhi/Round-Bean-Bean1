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
    public SpriteRenderer[] tiles;
    public Sprite[] groundImg;
    public SpriteRenderer[] seasonSign;

    public GameObject player;
    public GameObject hat;
    float cameraHalfWidth;
    SpriteRenderer temp; // for scrolling tiles

    private float startPos;
    public float distance;
    float tempdist = 0; // 계절 타일 변화와 장애물 알고리즘 변화 사이에 유예를 두기 위해 사용되는 변수
    bool isTempDistValid = false; // 계절 타일 변화와 장애물 알고리즘 변경 사이의 시점인 경우 True

    private int currentSeason = 0; // 계절 기록용 변수, 장애물 생성 가능여부 판정에 사용함. 0: spring, 1: summer, 2: autumn, 3: winter
    [SerializeField] int springTilesIndex; // 각 계절별 타일이 시작하는 위치. 타일이 하나씩인 경우 각각 0, 1, 2, 3.
    [SerializeField] int summerTilesIndex;
    [SerializeField] int autumnTilesIndex;
    [SerializeField] int winterTilesIndex;

    private int tilesStart;
    private int tilesEnd;

    private int obstacleCount = 0;
    private int hatCount = 0;
    private bool rockJudge = false;
    private bool cliffJudge = false;
    private int obstacleDelay = 0;
    private float obstacleChance = 0;
    private bool[] obstacleFlag = new bool[26]; // false: judgeable, true: already judged. set to false when declared

    [SerializeField] GameObject textGoForward; // 역행 금지 안내 텍스트

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
        // TODO: ground들의 기존 임시 이미지(ground뭐시기) 전부 교체할 것
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

        // check every tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            // set when to move the leftmost block that player already passed
            if (player.transform.position.x - cameraHalfWidth - 19 >= tiles[i].transform.position.x)
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
                && !obstacleFlag[i] && currentSeason >= 1) // and spawning obstacles is not judged yet, and season is after spring 
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

                // 가을 이후 생성 알고리즘
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
                                    SetTile(tiles[i], false); // hide tile = spawn cliff
                                    SoundManager.Instance.SFXPlay("BluffSpawned", cliffSpawnClip);
                                    Debug.Log("cliff selected");
                                }
                                else
                                {
                                    var rock = ObjectPool.GetObject();
                                    rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                                    SoundManager.Instance.SFXPlay("StoneSpawned", rockSpawnClip);
                                    Debug.Log("rock selected");
                                }
                                hatCount = 0;
                            }
                            else SetTile(tiles[i], false); // hide tile
                            hatCount = 0;
                        }
                        else if (!cliffJudge && rockJudge) // cliffJudge가 false고 rockjudge가 true인 경우에 대한 추가 처리
                        {
                            var rock = ObjectPool.GetObject();
                            rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                            SoundManager.Instance.SFXPlay("StoneSpawned", rockSpawnClip);
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
                if (obstacleCount > 0) // 가을 이후 절벽과의 동시 생성 여부 파악을 위해 미리 돌 판정
                {
                    rockJudge = ProbabilityRandom(obstacleChance);
                    Debug.Log("...judging rocks..." + rockJudge);
                    if (rockJudge)
                    {
                        var rock = ObjectPool.GetObject();
                        rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                        SoundManager.Instance.SFXPlay("StoneSpawned", rockSpawnClip);
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
    /// 계절별 타일 변경을 위한 함수. UpdateSeason에 의해 계절이 변경될 때만 호출됨.
    /// </summary>
    /// <param name="season">0: spring, 1: summer, 2: autumn, 3: winter. other values are not accepted.</param>
    /// <exception cref="Exception"></exception>
    private void SetSeasonTiles(int season) // change tile image range 
    {
        Debug.Log("season tile set to " + season);
        CreateSeasonSign(season, distance); // 계절 변경 안내 표지판 생성
        switch (season)
        {
            // summer: 20%/4m, autumn: 25%/2m, winter: 30%/1m
            case 0:
                tilesStart = springTilesIndex;
                tilesEnd = summerTilesIndex;
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
                tilesEnd = groundImg.Length;
                currentSeason = 3;
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
        Debug.Log("season obs set to " + season);
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
                obstacleDelay = -1;
                obstacleChance = 0.3f;
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
        if (distance < 50 * 1) newSeason = 0;
        else if (distance < 100 * 1) newSeason = 1;
        else if (distance < 155 * 1) newSeason = 2; // 가을 이후의 장애물 디버깅을 위해 거리를 늘리는 부분 (곱하는 수를 조정하면 됨)
        else if (distance < 215 * 1) newSeason = 3;
        else if (distance < 225 * 1) newSeason = 0; // spring2

        if (newSeason != currentSeason)
        {
            SetSeasonTiles(newSeason);
            tempdist = distance;
            isTempDistValid = true;
        }
        if (isTempDistValid && distance > tempdist + 12)
        {
            SetSeasonObs(newSeason);
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
        seasonSign[season].transform.position = new Vector3(distance+21, 1f, 0.5f); // z 값이 클수록 뒤로. groundImg는 1f, 나머지는 0f이다.
        seasonSign[season].gameObject.SetActive(true);
    }
}

/**
 * In-game Scene
 * infinite map scrolling & obstacle spawning
 * �� ���� ���� ����
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
    float tempdist = 0; // ���� Ÿ�� ��ȭ�� ��ֹ� �˰��� ��ȭ ���̿� ������ �α� ���� ���Ǵ� ����
    bool isTempDistValid = false; // ���� Ÿ�� ��ȭ�� ��ֹ� �˰��� ���� ������ ������ ��� True

    private int currentSeason = 0; // ���� ��Ͽ� ����, ��ֹ� ���� ���ɿ��� ������ �����. 0: spring, 1: summer, 2: autumn, 3: winter
    [SerializeField] int springTilesIndex; // �� ������ Ÿ���� �����ϴ� ��ġ. Ÿ���� �ϳ����� ��� ���� 0, 1, 2, 3.
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

    [SerializeField] GameObject textGoForward; // ���� ���� �ȳ� �ؽ�Ʈ

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
        tilesEnd = summerTilesIndex; // ���� ���� set, SetSeason() ��� �� ���ʿ��� ������ ����
        for (int i = 0; i < tiles.Length; i++) // ���� ���۰� ���ÿ� Ÿ�� �̹��� set
            tiles[i].sprite = groundImg[UnityEngine.Random.Range(tilesStart, tilesEnd)];
        // TODO: ground���� ���� �ӽ� �̹���(ground���ñ�) ���� ��ü�� ��
    }

    // Update is called once per frame
    void Update()
    {
        // �ִ� ���� �Ÿ� ������Ʈ
        if (player.transform.position.x > distance)
        {
            distance = player.transform.position.x - startPos;
        }

        // �ִ� ���� �Ÿ����� ���� �ڿ� ���� �� ( = ���� �õ� ��)
        if (player.transform.position.x < distance - 16 && !GameManager.Instance.gameOver) // �����ϴٰ� ���ӿ��� �� �ؽ�Ʈ�� ��ġ�� ���� ����
        {
            Time.timeScale = 0; // ���� �Ͻ�����. GameManager�� �Լ��� �߰��ؼ� �̿��ϴ� �� �� �ٶ����ұ�?
            textGoForward.SetActive(true); // �ȳ� �ؽ�Ʈ ǥ��
        }
        if (Time.timeScale == 0 && Input.GetKey(KeyCode.RightArrow))
        {
            Time.timeScale = 1; // �Ͻ����� ����
            textGoForward.SetActive(false); // �ȳ� �ؽ�Ʈ ����
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

            if (currentSeason == 0) obstacleFlag[i] = true; // ���� ��� ���� ���ʿ�, ���� �������ڸ��� �ٷ� ���� �� ������ ������ �õ��ϴ� ���� ����

            // spawning obstacles. is seperation to another file needed? and possible?
            if (player.transform.position.x + cameraHalfWidth * 0.59 <= tiles[i].transform.position.x
                && player.transform.position.x + cameraHalfWidth * 0.61 >= tiles[i].transform.position.x // when the block reaches at 80% of the display
                && !obstacleFlag[i] && currentSeason >= 1) // and spawning obstacles is not judged yet, and season is after spring 
            {
                obstacleFlag[i] = true; // �ߺ����� ����

                if (hatCount >= 3 && !GameManager.Instance.hatOn && obstacleCount > 0)
                // ��ֹ��� 3�� �̻� ��Ÿ���� ���� ���, ���� ���� ���ڸ� ���� ���� ���� ���. ��ֹ��� ���� ĭ�� ���� ����
                // ���ڸ� �� ���ȿ��� hatCount = 0
                {
                    bool hatJudge = ProbabilityRandom(0.8f);
                    if (hatJudge)
                    {
                        var newHat = Instantiate(hat, new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f), new Quaternion(0, 0, 0, 0));
                        GameManager.Instance.newHatGenerated = true; // ���� ���� �����Ǹ� newHatGenerated ������ ����
                        SoundManager.Instance.SFXPlay("HatGenerated", hatSpawnClip);

                        obstacleCount = obstacleDelay + 1; // ��ֹ� ������ ����
                        hatCount = 0;
                        Debug.Log("hat spawned");
                        break; // ���� ���� �� ���� ��ֹ� ������ �ʿ����
                    }
                    hatCount = 0; // else
                    Debug.Log("hat spawn failed");
                }

                // ���� ���� ���� �˰���
                if (currentSeason >= 2) 
                {
                #region afterAutumn
                    if (obstacleCount > 0) // 24.12.25 obstacleCount_rock�� obstacleCount_cliff ����
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
                        else if (!cliffJudge && rockJudge) // cliffJudge�� false�� rockjudge�� true�� ��쿡 ���� �߰� ó��
                        {
                            var rock = ObjectPool.GetObject();
                            rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                            SoundManager.Instance.SFXPlay("StoneSpawned", rockSpawnClip);
                            Debug.Log("rock selected");
                            hatCount = 0;
                        }
                        obstacleCount = obstacleDelay + 1; // �����ϵ� �����ϵ� 8m(4ĭ)���� �����ϹǷ� ������ ����.
                        if (!(cliffJudge || rockJudge || GameManager.Instance.hatOn)) hatCount++; // ��ֹ� ������ ������ ���, ���ڸ� �� ���� ������ hat count ����.
                        Debug.Log("hatCount: " + hatCount);
                        break;
                    }
                    else // ���� ������ ���� �ƴ� ��� (obstacleDelay�� �ɸ��� ���)
                    {
                        obstacleCount++;
                        break;
                    }
                #endregion
                }
                #region summer
                // ���� - ���� ������. 
                if (obstacleCount > 0) // ���� ���� �������� ���� ���� ���� �ľ��� ���� �̸� �� ����
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
    /// ������ Ÿ�� ������ ���� �Լ�. UpdateSeason�� ���� ������ ����� ���� ȣ���.
    /// </summary>
    /// <param name="season">0: spring, 1: summer, 2: autumn, 3: winter. other values are not accepted.</param>
    /// <exception cref="Exception"></exception>
    private void SetSeasonTiles(int season) // change tile image range 
    {
        Debug.Log("season tile set to " + season);
        CreateSeasonSign(season, distance); // ���� ���� �ȳ� ǥ���� ����
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
    /// ������ ��ֹ� ���� ������ ���� �Լ�. UpdateSeason�� ���� ������ ����� ���� ȣ��Ǹ�, SetSeasonTiles ȣ�� ���Ŀ� ȣ���.
    /// ���� Ÿ�ϼ� ����� ��ֹ� �˰��� ���� ���̿� ������ �α� ���Ͽ� �и�.
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
                obstacleDelay = -4; // ��ֹ��� ������ ��� ���� 4ĭ ������ �������� ����. 
                obstacleChance = 0.2f; // ������ ���� Ȯ�� ������ ���⼭ �� obstacleChance �� ���� (0 �̻� 1 ����)
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
    /// ���� ��ȭ�� �Լ��� �и�. ���� string�� �̿��ϴ� ��� ��� currentSeason ���� �ϳ��� ����.
    /// �� �����Ӹ��� �Ÿ��� ���� ������ �ݺ��Ͽ� set�ϴ� ���
    /// ������ �ٲ�� ��쿡�� ���Ӱ� set�ϵ��� ��.
    /// �� �����Ӹ��� ȣ��.
    /// </summary>
    /// <param name="distance">���� ���� �÷��̿��� �ִ� ���� �Ÿ�</param>
    private void UpdateSeason(float distance)
    {
        int newSeason = 0;

        // 24.12.28 ���� �� 50m, ���� 50m, ���� 55m, �ܿ� 60m, ��2 10m
        if (distance < 50 * 1) newSeason = 0;
        else if (distance < 100 * 1) newSeason = 1;
        else if (distance < 155 * 1) newSeason = 2; // ���� ������ ��ֹ� ������� ���� �Ÿ��� �ø��� �κ� (���ϴ� ���� �����ϸ� ��)
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
    /// ���� ���� �ȳ� ǥ���� ���� �Լ�.
    /// ������ �ٲ�� ��ġ���� �˾Ƽ� �����ǵ��� ��.
    /// </summary>
    /// <param name="season">������ �ش��ϴ� ǥ������ ����� ����</param>
    private void CreateSeasonSign(int season, float distance)
    {
        seasonSign[season].transform.position = new Vector3(distance+21, 1f, 0.5f); // z ���� Ŭ���� �ڷ�. groundImg�� 1f, �������� 0f�̴�.
        seasonSign[season].gameObject.SetActive(true);
    }
}

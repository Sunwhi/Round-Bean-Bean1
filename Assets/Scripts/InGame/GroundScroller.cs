/**
 * In-game Scene
 * infinite map scrolling & obstacle spawning
 * �� ���� ���� ����
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
    public SpriteRenderer[] seasonSign;

    public GameObject player;
    public GameObject hat;
    float cameraHalfWidth;
    SpriteRenderer temp; // for scrolling tiles

    private float startPos;
    public float distance;

    private int currentSeason = 0; // ���� ��Ͽ� ����, ��ֹ� ���� ���ɿ��� ������ �����. 0: spring, 1: summer, 2: autumn, 3: winter
    private int springTilesIndex = 0; // ���߿� ������ Ÿ�� ���� �� ���� �� ���.. ���� ���� ����� �ƴ�����. �� ���� ��� ����غ���
    private int summerTilesIndex = 1;
    private int autumnTilesIndex = 2;
    private int winterTilesIndex = 3;

    private int tilesStart = 0;
    private int tilesEnd = 1;

    private int obstacleCount = 0;
    private int hatCount = 0;
    private bool rockJudge = false;
    private bool cliffJudge = false;
    private int obstacleDelay = 0;
    private float obstacleChance = 0;
    private bool[] obstacleFlag = new bool[21]; // false: judgeable, true: already judged. set to false when declared

    [SerializeField] GameObject textGoForward; // ���� ���� �ȳ� �ؽ�Ʈ

    void Start()
    {
        temp = tiles[0];
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        startPos = player.transform.position.x;
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

            if (tiles[i].sprite == groundImg[0]) obstacleFlag[i] = true; // ���� ��� ���� ���ʿ�, ���� �������ڸ��� �ٷ� ���� �� ������ ������ �õ��ϴ� ���� ����

            // spawning obstacles. is seperation to another file needed? and possible?
            if (player.transform.position.x + cameraHalfWidth * 0.59 <= tiles[i].transform.position.x
                && player.transform.position.x + cameraHalfWidth * 0.61 >= tiles[i].transform.position.x // when the block reaches at 80% of the display
                && !obstacleFlag[i] && currentSeason >= 1) // and spawning obstacles is not judged yet, and season is after spring 
            {
                obstacleFlag[i] = true; // �ߺ����� ����

                if (hatCount >= 3 && !GameManager.Instance.hatOn && obstacleCount > 0)
                // ��ֹ��� 3�� �̻� ��Ÿ���� ���� ���, ���� ���� ���ڸ� ���� ���� ���� ���. ��ֹ��� ���� ĭ�� ���� ����
                // TODO: hatCount�� ������ ���ڸ� �� ���¿����� ����Ǵ°�?
                // ���� �� ���ڸ� �����ڸ��� ���ڰ� ������ ��Ȳ�� �߻�. �켱�� ���������� ��. �����ؾ� �ϴ� ��� �Ʒ��� if (!GameManager.Instance.hatOn) ���� ���� ��.
                {
                    bool hatJudge = ProbabilityRandom(0.8f);
                    if (hatJudge)
                    {
                        var newHat = Instantiate(hat, new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f), new Quaternion(0, 0, 0, 0));
                        obstacleCount = obstacleDelay + 1; // ������ ����
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
                                    Debug.Log("cliff selected");
                                }
                                else
                                {
                                    var rock = ObjectPool.GetObject();
                                    rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                                    Debug.Log("rock selected");
                                }
                                hatCount = 0;
                                break;
                            }
                            else SetTile(tiles[i], false); // hide tile
                            hatCount = 0;
                            break;
                        }
                        else if (!cliffJudge && rockJudge) // cliffJudge�� false�� rockjudge�� true�� ��츦 ó�� ���߾ �߰���.
                        {
                            var rock = ObjectPool.GetObject();
                            rock.transform.position = new Vector2(tiles[i].transform.position.x + 0.5f, -0.5f); // spawn rock
                            Debug.Log("rock selected");
                            hatCount = 0;
                            break;
                        }
                        obstacleCount = obstacleDelay + 1; // �����ϵ� �����ϵ� 8m(4ĭ)���� �����ϹǷ� ������ ����.
                        if (!GameManager.Instance.hatOn) hatCount++; // ��ֹ� ������ ������ ���, ���ڸ� �� ���� ������ hat count ����.
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
    /// ���� ������ ���� �Լ�. UpdateSeason�� ���� ������ ����� ���� ȣ���.
    /// </summary>
    /// <param name="season">0: spring, 1: summer, 2: autumn, 3: winter. other values are not accepted.</param>
    /// <exception cref="Exception"></exception>
    private void SetSeason(int season) // change tile image range 
    {
        Debug.Log("season set to " + season);
        CreateSeasonSign(season, distance); // ���� ���� �ȳ� ǥ���� ����
        switch (season)
        {

            // summer: 20%/4m, autumn: 25%/2m, winter: 30%/1m
            case 0:
                tilesStart = springTilesIndex;
                tilesEnd = summerTilesIndex;
                currentSeason = 0;
                obstacleChance = 0f; // ������ ���� Ȯ�� ������ ���⼭ �� obstacleChance �� ���� (0 �̻� 1 ����)
                break;
            case 1:
                tilesStart = summerTilesIndex;
                tilesEnd = autumnTilesIndex;
                currentSeason = 1;
                obstacleDelay = -4; // ��ֹ��� ������ ��� ���� 4ĭ ������ �������� ����. 
                obstacleChance = 0.2f;
                break;
            case 2:
                tilesStart = autumnTilesIndex;
                tilesEnd = winterTilesIndex;
                currentSeason = 2;
                obstacleDelay = -2; 
                obstacleChance = 0.25f;
                break;
            case 3:
                tilesStart = winterTilesIndex;
                tilesEnd = groundImg.Length;
                currentSeason = 3;
                obstacleDelay = -1; 
                obstacleChance = 0.3f;
                break;
            default:
                throw new Exception("Season name not valid");
        }
    }


    /// <summary>
    /// ���� ��ȭ�� �Լ��� �и�. ���� string�� �̿��ϴ� ��� ��� currentSeason ���� �ϳ��� ����.
    /// �� �����Ӹ��� �Ÿ��� ���� ������ �ݺ��Ͽ� set�ϴ� ���
    /// ������ �ٲ�� ��쿡�� ���Ӱ� set�ϵ��� ��.
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
            currentSeason = newSeason;
            SetSeason(currentSeason);
        }
    }

    /// <summary>
    /// ���� ���� �ȳ� ǥ���� ���� �Լ�.
    /// ������ �ٲ�� ��ġ���� �˾Ƽ� �����ǵ��� ��.
    /// </summary>
    /// <param name="season">������ �ش��ϴ� ǥ������ ����� ����</param>
    private void CreateSeasonSign(int season, float distance)
    {
        seasonSign[season].transform.position = new Vector3(distance+11, 1f, 0.5f); // z ���� Ŭ���� �ڷ�. groundImg�� 1f, �������� 0f�̴�.
        seasonSign[season].gameObject.SetActive(true);
    }
}

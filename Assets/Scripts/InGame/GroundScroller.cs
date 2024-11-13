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

    private int seasonNow = 0; // ���� ��Ͽ� ����, ��ֹ� ���� ���ɿ��� ������ �����. 0: spring, 1: summer, 2: autumn, 3: winter, 4: spring2(�̻��)
    private int springTilesIndex = 0; // ���߿� ������ Ÿ�� ���� �� ���� �� ���.. ���� ���� ����� �ƴ�����. �� ���� ��� ����غ���
    private int summerTilesIndex = 1;
    private int autumnTilesIndex = 2;
    private int winterTilesIndex = 3;

    private int tilesStart = 0;
    private int tilesEnd = 1;

    private int obstacleCount = 0;
    private int obstacleDelay = 0;
    private float obstacleChance = 0;
    private bool[] obstacleFlag = new bool[17]; // false: judgeable, true: already judged. set to false when declared



    // Start is called before the first frame update
    void Start()
    {
        temp = tiles[0];
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        startPos = player.transform.position.x;

        //StartCoroutine(DistanceLog());
    }
    // 1�ʸ��� �̵��Ÿ� �α׿� ��� (������)
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
        distance = player.transform.position.x - startPos;
        // summer: 10m, autumn: 21m, winter: 33m, spring2: 45m
        if (distance < 10 * 2) SetSeason("spring");
        else if (distance < 21 * 2) SetSeason("summer");
        else if (distance < 33 * 200) SetSeason("autumn"); // ���� ������ ��ֹ� ������� ���� ��� �Ÿ��� �÷� �ξ���
        else if (distance < 45 * 2) SetSeason("winter");
        else if (distance < 50 * 2) SetSeason("spring"); // spring2

        // check every tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            // set when to move the leftmost block that player already passed
            if (player.transform.position.x - cameraHalfWidth - 7 >= tiles[i].transform.position.x) // cameraHalfWidth - 11 -> cameraHalfWidth - 7
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



            // spawning obstacles. is seperation to another file needed? and possible?
            // summer: 0.2/4m, autumn: 0.25/2m, winter: 0.3/1m
            if (player.transform.position.x + cameraHalfWidth * 0.59 <= tiles[i].transform.position.x
                && player.transform.position.x + cameraHalfWidth * 0.61 >= tiles[i].transform.position.x // when the block reaches at 80% of the display
                && !obstacleFlag[i] && seasonNow >= 1) // and spawning obstacles is not judged yet, and season is after spring
            {
                // ������ ���� ���� ���� �˰��� �κ�. 
                if (seasonNow >= 2) 
                {
                    #region autumn_spawnCliffsAndRocks
                    obstacleFlag[i] = true; // true: have been judged
                    bool cliffJudge = ProbabilityRandom(obstacleChance);
                    if (obstacleCount > 0 && cliffJudge)
                    {
                        bool cliffOrRock = ProbabilityRandom(0.5f); // 50% probability
                        if (cliffOrRock) // cliff first, when true
                        {
                            SetTile(tiles[i], false); // hide tile 
                            Instantiate(rock, new Vector2(tiles[i].transform.position.x + 10, -0.5f), new Quaternion(0, 0, 0, 0)); // spawn rock after 5 block
                            Debug.Log("cliff first! spawned at " + i);
                        }
                        else // rock first, when true
                        {
                            Instantiate(rock, new Vector2(tiles[i].transform.position.x, -0.5f), new Quaternion(0, 0, 0, 0)); // spawn rock after 5 block
                            try
                            {
                                SetTile(tiles[i + 5], false); // hide tile
                            }
                            catch (IndexOutOfRangeException)
                            {
                                SetTile(tiles[i - 12], false); // hide tile
                            }
                            Debug.Log("rock first! spawned at " + i);
                        }

                        // obstacles do not spawn more until following 5 blocks
                        obstacleCount = -5;

                        Debug.Log("obstacleCount : " + obstacleCount);
                        break;
                    }
                    else
                    {
                        obstacleCount++;
                        Debug.Log("obstacleCount : " + obstacleCount);
                        break;
                    }
                    #endregion
                }
                #region summer_spawnOnlyRocks
                // ���� - ���� ������. �� ĭ���� ������ �ƴ� �� ĭ ���� �� ���� �� ���� �� ĭ�� ���� �����ϴ� ���.
                obstacleFlag[i] = true; // true: have been judged
                bool rockJudge = ProbabilityRandom(obstacleChance);
                if (obstacleCount > 0 && rockJudge)
                {
                    Instantiate(rock, new Vector2(tiles[i].transform.position.x + 2, -0.5f), new Quaternion(0, 0, 0, 0)); // spawn rock
                    obstacleCount = obstacleDelay;
                    Debug.Log("obstacleCount: " + obstacleCount);
                }
                else
                {
                    obstacleCount++;
                    Debug.Log("obstacleCount: " + obstacleCount);
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
                obstacleChance = 0f; // ������ ���� Ȯ�� ������ ���⼭ �� obstacleChance �� ���� (1 ����)
                break;
            case "summer":
                tilesStart = summerTilesIndex;
                tilesEnd = autumnTilesIndex;
                seasonNow = 1;
                obstacleDelay = -4; // ��ֹ��� ������ ��� ���� 4ĭ ������ �������� ����. 
                obstacleChance = 0.2f;
                break;
            case "autumn":
                tilesStart = autumnTilesIndex;
                tilesEnd = winterTilesIndex;
                seasonNow = 2;
                obstacleDelay = -2;
                obstacleChance = 0.25f;
                break;
            case "winter":
                tilesStart = winterTilesIndex;
                tilesEnd = groundImg.Length;
                seasonNow = 3;
                obstacleDelay = -1;
                obstacleChance = 0.3f;
                break;
            //case "spring2": // �����غ��� �ʿ䰡 ����. �ϴ� ��� ����
            //    tilesStart = springTilesIndex;
            //    tilesEnd = summerTilesIndex;
            //    seasonNow = 4;
            //    break;
            default:
                throw new Exception("Season name not valid");
        }
    }

}

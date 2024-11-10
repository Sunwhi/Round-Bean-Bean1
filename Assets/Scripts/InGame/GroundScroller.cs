/**
 * In-game Scene
 * infinite map scrolling
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public SpriteRenderer[] tiles;
    public Sprite[] groundImg;
    public GameObject player;
    float cameraHalfWidth; 
    SpriteRenderer temp;

    // Start is called before the first frame update
    void Start()
    {
        temp = tiles[0];
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        Debug.Log(cameraHalfWidth);
    }

    // Update is called once per frame
    void Update()
    {
        // check every tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            // set when to move the leftmost block that player already passed
            if(player.transform.position.x - cameraHalfWidth-6 >= tiles[i].transform.position.x) // cameraHalfWidth - 11 -> cameraHalfWidth - 7
            {
                for(int q=0; q<tiles.Length; q++)
                {
                    // find the location of the rightmost block
                    if(temp.transform.position.x < tiles[q].transform.position.x)
                    {
                        temp = tiles[q];
                    }
                }
                // move block next to the rightmost block
                tiles[i].transform.position = new Vector2(temp.transform.position.x + 2, -2f);
                tiles[i].gameObject.GetComponent<Collider2D>().enabled = true; // tile collider enable, just in case disabled last time
                tiles[i].gameObject.SetActive(true); // set visible, just in case disabled last time

                // if ground img has variations, change img randomly
                tiles[i].sprite = groundImg[UnityEngine.Random.Range(0,groundImg.Length)];
            }
        }
    }

    
}

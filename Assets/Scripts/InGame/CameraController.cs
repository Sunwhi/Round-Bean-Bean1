/**
 * In-game scene
 * controller for main camera position
 * move along the player's position.x
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float cameraHalfWidth, cameraHalfHeight;

    // Start is called before the first frame update
    void Start()
    {
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        //cameraHalfHeight = Camera.main.orthographicSize;
    }

    public Transform player;

    // Update is called once per frame
    void LateUpdate()
    {
        // fix camera position to player
        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
    }
}

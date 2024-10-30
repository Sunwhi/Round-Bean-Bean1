using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private GameObject circle;
    private Vector3 cameraPosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        cameraPosition = new Vector3(circle.transform.position.x, circle.transform.position.y + 1, -10);
        transform.position = cameraPosition;
    }
}

using UnityEngine;
using System.Collections;

public class ObjectDestructor : MonoBehaviour
{
    public float destructionDelay = 5.0f;

    void Start()
    {
        Destroy(gameObject, destructionDelay);
    }
}

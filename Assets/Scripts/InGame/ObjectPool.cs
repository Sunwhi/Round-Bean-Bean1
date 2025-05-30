using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public int defaultCapacity = 10;
    public int maxPoolSize = 15;

    [SerializeField]
    public GameObject rockPrefab;

    [SerializeField]
    public Sprite[] rockImg;
    Queue<Rock> rockQueue = new Queue<Rock>();
    private void Awake()
    {
        instance = this;

        Init();
    }

    private void Init()
    {
        for (int i = 0; i < defaultCapacity; i++) 
        {
            rockQueue.Enqueue(CreateObject());
        }
    }

    // 돌부리 생성
    private Rock CreateObject()
    {
        var newObj = Instantiate(rockPrefab).GetComponent<Rock>();
        newObj.spriteRenderer.sprite = rockImg[0]; // 기본 돌
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(null);
        return newObj;
    }

    // 사용
    public static Rock GetObject()
    {
        if(instance.rockQueue.Count > 0)
        {
            var obj = instance.rockQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            Debug.Log("got an object");
            return obj;
        }
        else
        {
            var newObj = instance.CreateObject();
            newObj.gameObject.SetActive(true);
            newObj.transform.SetParent(null);
            Debug.Log("got an object");
            return newObj;
        }
    }

    // 반환
    public static void ReturnObject(Rock poolObj)
    {
        poolObj.gameObject.SetActive(false);
        instance.rockQueue.Enqueue(poolObj);
    }

    // 삭제
    public void DestroyObject(GameObject poolObj)
    {
        Destroy(poolObj);
    }
}

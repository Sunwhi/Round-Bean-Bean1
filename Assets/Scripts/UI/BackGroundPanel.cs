using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BackGroundPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject clearRecordPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    // 업적창 바깥을 누르면 업적창 꺼짐
    public void OnPointerDown(PointerEventData eventData)
    {
        if(clearRecordPanel.activeSelf)
        {
            clearRecordPanel.SetActive(false);
        }
    }
}

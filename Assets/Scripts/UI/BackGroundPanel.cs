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
    // ����â �ٱ��� ������ ����â ����
    public void OnPointerDown(PointerEventData eventData)
    {
        if(clearRecordPanel.activeSelf)
        {
            clearRecordPanel.SetActive(false);
        }
    }
}

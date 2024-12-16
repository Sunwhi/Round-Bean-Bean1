using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleasRecordBtn : MonoBehaviour
{
    [SerializeField] GameObject ClearRecordPanel;
    public void ClearRecordBtn()
    {
        if (!ClearRecordPanel.activeSelf)
        {
            ClearRecordPanel.SetActive(true);
        }
    }
    public void ExitBtn()
    {
        if(ClearRecordPanel.activeSelf)
        {
            ClearRecordPanel.SetActive(false);
        }
    }
}

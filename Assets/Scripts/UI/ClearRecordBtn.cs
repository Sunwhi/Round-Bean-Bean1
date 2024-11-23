using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleasRecordBtn : MonoBehaviour
{
    [SerializeField] GameObject ClearRecordPanel;
    public void ClearRecordBtn()
    {
        ClearRecordPanel.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanel : MonoBehaviour
{
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;
    [SerializeField] GameObject panel3;
    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    int activePanel = 1;
    public void RightArrow()
    {
        switch(activePanel)
        {
            case 1:
                panel1.SetActive(false);
                panel2.SetActive(true);
                activePanel = 2;
                break;
            case 2:
                panel2.SetActive(false);
                panel3.SetActive(true);
                activePanel = 3;
                break;
        }
    }
    public void LeftArrow()
    {
        switch(activePanel)
        {
            case 2:
                panel1.SetActive(true);
                panel2.SetActive(false);
                activePanel = 1;
                break;
            case 3:
                panel2.SetActive(true);
                panel3.SetActive(false);
                activePanel = 2;
                break;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(activePanel)
        {
            case 1:
                leftArrow.SetActive(false);
                break;
            case 2:
                leftArrow.SetActive(true);
                rightArrow.SetActive(true);
                break;
            case 3:
                rightArrow.SetActive(false);
                break;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanel : MonoBehaviour
{
    [SerializeField] GameObject panel1;
    [SerializeField] GameObject panel2;
    [SerializeField] GameObject panel3;
    [SerializeField] GameObject panel4;
    [SerializeField] GameObject panel5;
    [SerializeField] GameObject panel6;
    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    public AudioClip arrowSfx;
    int activePanel = 1;
    public void RightArrow()
    {
        SoundManager.Instance.SFXPlay("arrowClick",arrowSfx);
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
            case 3:
                panel3.SetActive(false);
                panel4.SetActive(true);
                activePanel = 4;
                break;
            case 4:
                panel4.SetActive(false);
                panel5.SetActive(true);
                activePanel = 5;
                break;
            case 5:
                panel5.SetActive(false);
                panel6.SetActive(true);
                activePanel = 6;
                break;
        }
    }
    public void LeftArrow()
    {
        SoundManager.Instance.SFXPlay("arrowClick", arrowSfx);
        switch (activePanel)
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
            case 4:
                panel3.SetActive(true);
                panel4.SetActive(false);
                activePanel = 3;
                break;
            case 5:
                panel4.SetActive(true);
                panel5.SetActive(false);
                activePanel = 4;
                break;
            case 6:
                panel5.SetActive(true);
                panel6.SetActive(false);
                activePanel = 5;
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
            case 5:
                leftArrow.SetActive(true);
                rightArrow.SetActive(true);
                break;
            case 6:
                rightArrow.SetActive(false);
                break;

        }
    }
}

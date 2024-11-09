using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private float screenWidthHalf;
    [SerializeField] GameObject Protracter;
    [SerializeField] GameObject Speedometer;
    [SerializeField] GameObject mainCamera;
    private float mainCameraXPos;
    // Start is called before the first frame update
    void Start()
    {
        Protracter.SetActive(false);
        Speedometer.SetActive(false);
        mainCameraXPos = mainCamera.transform.position.x;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch1 = Input.GetTouch(0);
            //Touch touch2 = Input.GetTouch(1);

            screenWidthHalf = Screen.width / 2; 

            Vector2 touch1Position = touch1.position; // c#에선 지역변수에 접근지정자 못 붙임. private 못 붙임!
            //mainCameraXPos - screenWidthHalf < touch1Position.x && touch1Position.x < mainCameraXPos

            if (touch1.phase == TouchPhase.Began ||  touch1.phase == TouchPhase.Moved)
            {
                if(touch1Position.x < screenWidthHalf)
                {
                    Protracter.transform.position = Camera.main.ScreenToWorldPoint(touch1Position);
                    Vector3 newPosition = Protracter.transform.position;
                    newPosition.z = 0;
                    Protracter.transform.position = newPosition;
                    Protracter.SetActive(true);
                }
                else
                {
                    Speedometer.transform.position = Camera.main.ScreenToWorldPoint(touch1Position);
                    Vector3 newPosition = Speedometer.transform.position;
                    newPosition.z = 0;
                    Speedometer.transform.position = newPosition;
                    Speedometer.SetActive(true);
                }
            }
        }
     
    }
}

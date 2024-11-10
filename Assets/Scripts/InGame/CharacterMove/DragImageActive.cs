using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/*
 화면 왼쪽, 오른쪽 터치했을때 각도계, 속도계 이미지가 나타나도록 하는 스크립트
 */
public class NewBehaviourScript : MonoBehaviour
{
    private float screenWidthHalf;
    [SerializeField] GameObject Protracter;
    [SerializeField] GameObject Speedometer;
    private float mainCameraXPos;
    // Start is called before the first frame update
    void Start()
    {
        Protracter.SetActive(false);
        Speedometer.SetActive(false);
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

                // 각도계 표시, 화면을 반으로 나눴을 때 왼쪽부분을 터치하면 이미지가 나타난다.
                if(touch1Position.x < screenWidthHalf)
                {
                    // 화면상에 터치포인트를 월드 좌표계로 변환해서 나타내기. z축은 0으로 바꾼다.
                    Protracter.transform.position = Camera.main.ScreenToWorldPoint(touch1Position);
                    Vector3 newPosition = Protracter.transform.position;
                    newPosition.z = 0;
                    Protracter.transform.position = newPosition;

                    Protracter.SetActive(true);
                }

                // 속도계 표시, 화면을 반으로 나눴을 때 오른쪽부분을 터치하면 이미지가 나타난다.
                else
                {
                    // 화면상에 터치포인트를 월드 좌표계로 변환해서 나타내기. z축은 0으로 바꾼다.
                    Speedometer.transform.position = Camera.main.ScreenToWorldPoint(touch1Position);
                    Vector3 newPosition = Speedometer.transform.position;
                    newPosition.z = 0;
                    Speedometer.transform.position = newPosition;

                    Speedometer.SetActive(true);
                }
            }
            // 화면에서 손을 떼었을 때 이미지 사라지게함.
            if (touch1.phase == TouchPhase.Ended) Protracter.SetActive(false);
            if (touch1.phase == TouchPhase.Ended) Speedometer.SetActive(false);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDragImage : MonoBehaviour
{
    private float screenWidthHalf;
    [SerializeField] GameObject Protracter;
    [SerializeField] GameObject Speedometer;
    private RectTransform protracterRect;
    private RectTransform speedometerRect;

    // Start is called before the first frame update
    void Start()
    {
        // UI 이미지를 RectTransform으로
        protracterRect = Protracter.GetComponent<RectTransform>();
        speedometerRect = Speedometer.GetComponent<RectTransform>();

        Protracter.SetActive(false);
        Speedometer.SetActive(false);
        screenWidthHalf = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                DragImage(touch);
            }
        }
    }
    private void DragImage(Touch touch1)
    {
        Vector2 touch1Position = touch1.position;

        if (touch1.phase == TouchPhase.Began || touch1.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Stationary)
        {
            Debug.Log("1");
            // 스크린 좌표를 UI 요소에 맞게 변환
            Vector2 uiPosition = touch1Position;

            if (touch1Position.x < screenWidthHalf) // 왼쪽 터치: Protracter
            {
                protracterRect.position = uiPosition;
                Protracter.SetActive(true);
            }
            else // 오른쪽 터치: Speedometer
            {
                speedometerRect.position = uiPosition;
                Speedometer.SetActive(true);
            }
        }

        // 화면에서 손을 떼었을 때 이미지 사라지게 함
        if (touch1.phase == TouchPhase.Ended)
        {
            Protracter.SetActive(false);
            Speedometer.SetActive(false);
        }
    }
}

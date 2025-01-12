using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal;
using UnityEngine;

public class CanvasDragImage : MonoBehaviour
{
    private float screenWidthHalf;
    [SerializeField] GameObject Protracter;
    [SerializeField] GameObject Speedometer;
    private GameObject optionBtn; 
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

        optionBtn = GameObject.Find("OptionBtn");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameOver || GameManager.Instance.gameClear)
        {
            Protracter.SetActive(false);
            Speedometer.SetActive(false);
        }
        if (!GameManager.Instance.gameOver && !GameManager.Instance.gameClear)
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
    }
    private void DragImage(Touch touch1)
    {
        Vector2 touch1Position = touch1.position;
        Vector2 optionBtnPosition = optionBtn.transform.position;

        if (touch1.phase == TouchPhase.Began || touch1.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Stationary)
        {
            // 스크린 좌표를 UI 요소에 맞게 변환
            Vector2 uiPosition = touch1Position;
            //Debug.Log(uiPosition);
            
            if (touch1Position.x < screenWidthHalf) // 왼쪽 터치: Protracter
            {
                protracterRect.position = uiPosition;
                Protracter.SetActive(true);
            }
            else // 오른쪽 터치: Speedometer
            {
                // 옵션창 부근은 Speedometer터치가 되지 않도록.
                if (!(uiPosition.x > optionBtnPosition.x - 50 && uiPosition.y >optionBtnPosition.y - 50))
                {
                    speedometerRect.position = uiPosition;
                    Speedometer.SetActive(true);
                }
                else
                {
                    Speedometer.SetActive(false);
                }
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

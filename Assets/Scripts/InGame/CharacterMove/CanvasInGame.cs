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
        // UI �̹����� RectTransform����
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
            // ��ũ�� ��ǥ�� UI ��ҿ� �°� ��ȯ
            Vector2 uiPosition = touch1Position;
            //Debug.Log(uiPosition);
            
            if (touch1Position.x < screenWidthHalf) // ���� ��ġ: Protracter
            {
                protracterRect.position = uiPosition;
                Protracter.SetActive(true);
            }
            else // ������ ��ġ: Speedometer
            {
                // �ɼ�â �α��� Speedometer��ġ�� ���� �ʵ���.
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

        // ȭ�鿡�� ���� ������ �� �̹��� ������� ��
        if (touch1.phase == TouchPhase.Ended)
        {
            Protracter.SetActive(false);
            Speedometer.SetActive(false);
        }
    }
}

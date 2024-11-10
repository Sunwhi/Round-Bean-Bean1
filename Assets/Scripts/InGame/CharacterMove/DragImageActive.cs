using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
/*
 ȭ�� ����, ������ ��ġ������ ������, �ӵ��� �̹����� ��Ÿ������ �ϴ� ��ũ��Ʈ
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

            Vector2 touch1Position = touch1.position; // c#���� ���������� ���������� �� ����. private �� ����!
            //mainCameraXPos - screenWidthHalf < touch1Position.x && touch1Position.x < mainCameraXPos

            if (touch1.phase == TouchPhase.Began ||  touch1.phase == TouchPhase.Moved)
            {

                // ������ ǥ��, ȭ���� ������ ������ �� ���ʺκ��� ��ġ�ϸ� �̹����� ��Ÿ����.
                if(touch1Position.x < screenWidthHalf)
                {
                    // ȭ��� ��ġ����Ʈ�� ���� ��ǥ��� ��ȯ�ؼ� ��Ÿ����. z���� 0���� �ٲ۴�.
                    Protracter.transform.position = Camera.main.ScreenToWorldPoint(touch1Position);
                    Vector3 newPosition = Protracter.transform.position;
                    newPosition.z = 0;
                    Protracter.transform.position = newPosition;

                    Protracter.SetActive(true);
                }

                // �ӵ��� ǥ��, ȭ���� ������ ������ �� �����ʺκ��� ��ġ�ϸ� �̹����� ��Ÿ����.
                else
                {
                    // ȭ��� ��ġ����Ʈ�� ���� ��ǥ��� ��ȯ�ؼ� ��Ÿ����. z���� 0���� �ٲ۴�.
                    Speedometer.transform.position = Camera.main.ScreenToWorldPoint(touch1Position);
                    Vector3 newPosition = Speedometer.transform.position;
                    newPosition.z = 0;
                    Speedometer.transform.position = newPosition;

                    Speedometer.SetActive(true);
                }
            }
            // ȭ�鿡�� ���� ������ �� �̹��� ���������.
            if (touch1.phase == TouchPhase.Ended) Protracter.SetActive(false);
            if (touch1.phase == TouchPhase.Ended) Speedometer.SetActive(false);
        }

    }
}

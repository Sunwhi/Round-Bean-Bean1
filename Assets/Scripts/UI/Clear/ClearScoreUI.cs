using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;

public class ClearScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] GameObject newRecordText;
    [SerializeField] GameObject clearText;
    [SerializeField] GameObject clearBackgroundPanel;
    [SerializeField] GameObject homeBtn;
    [SerializeField] GameObject recordBtn;
    [SerializeField] GameObject playAgainBtn;
    [SerializeField] GameObject popperLeft;
    [SerializeField] GameObject popperRight;

    private Vector2 startPosition;
    [SerializeField] Vector2 endPosition;
    private float animationDuration = 3f;
    private float startScale = 2f;
    private float endScale = 4f;
    private float timer;

    /*Popper 관련*/
    //1은 왼쪽, 2는 오른쪽
    Vector3 popperZeroScale = new Vector3(0, 0, 0);
    //pingpong의 max와 minscale, popper그림이 커졌다 작아졌다 할때 끝의 scale값들
    Vector3 popperMaxScale1 = new Vector3(4, 3, 3);
    Vector3 popperMaxScale2 = new Vector3(4, 4, 3);
    Vector3 popperMinScale1 = new Vector3(3, 2, 1);
    Vector3 popperMinScale2 = new Vector3(2, 3, 1);
    //pingpong이 끝나고나서의 popper scalse
    Vector3 popperLastScale1;
    Vector3 popperLastScale2;

    private float timeElapsed1 = 0f;
    private float timeElapsed2 = 0f;   
    private float popperDisappearDuration = 0.6f; // popper가 사라지는 애니메이션의 속도
    private float popperPingPongDuration = 0.6f; // popper가 PingPong하는 속도(커졌다 작아졌다하는)
    private float popperPingPongTime = 4.5f; // popper PingPong의 지속시간(커졌다 작아지는 애니메이션의 지속 시간)

    // Start is called before the first frame update
    void Start()
    {
        startPosition = finalScoreText.rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.gameClear)
        {
            //popperLeft.SetActive(true);
            //PopperAnimation();
            StartCoroutine(PopperAnimation());
        }
        if(timer < animationDuration && GameManager.Instance.gameClear && !clearText.activeSelf)
        {
            // 시간 지남에 따라 스코어 커지면서 가운데로 오게
            timer += Time.deltaTime;
            
            finalScoreText.rectTransform.localPosition = Vector2.Lerp(startPosition, endPosition, timer / animationDuration);
             
            float scale = Mathf.Lerp(startScale,endScale,timer / animationDuration);
            finalScoreText.rectTransform.localScale = scale * Vector2.one;

            // 클리어 배경 화면 어둡게, panel이용해서 
            clearBackgroundPanel.SetActive(true);
            Color color = clearBackgroundPanel.GetComponent<Image>().color;
            color.a = Mathf.Lerp(0, 0.7f, timer / animationDuration);
            clearBackgroundPanel.GetComponent<Image>().color = color;

            if(!homeBtn.activeSelf)
            {
                StartCoroutine(clearBtn());
            }
            if (!newRecordText.activeSelf && finalScoreText.rectTransform.localPosition.y == 200)
            {
                // 신기록이라면 NEW RECORD!!
                if (GameManager.Instance.isNewRecord)
                {
                    newRecordText.SetActive(true);
                }
            }
        }

    }
    private float timeElapsed3;
    IEnumerator PopperAnimation()
    {
        // popper가 커졌다 작아졌다 하는 애니메이션
        if(timeElapsed1 < popperPingPongTime)
        {
            timeElapsed1 += Time.deltaTime;

            float t = Mathf.PingPong(Time.time / popperPingPongDuration, 1);

            popperLeft.transform.localScale = Vector3.Lerp(popperMaxScale1, popperMinScale1, t);
            popperRight.transform.localScale = Vector3.Lerp(popperMaxScale2, popperMinScale2, t);

            popperLastScale1 = popperLeft.transform.localScale;
            popperLastScale2 = popperRight.transform.localScale;
        }
        // popper가 작아지면서 사라지는 애니메이션
        if(timeElapsed1 > popperPingPongTime && timeElapsed2 < popperDisappearDuration)
        {
            timeElapsed2 += Time.deltaTime;
            
            float t = timeElapsed2 / popperDisappearDuration;
            
            popperLeft.transform.localScale = Vector3.Lerp(popperLastScale1, popperZeroScale, t);
            popperRight.transform.localScale = Vector3.Lerp(popperLastScale2, popperZeroScale, t);
        }
        yield return null;
    }
    //클리어 시 8초 후(점수 크게 나타나지고 얼마후) 홈 버튼과 업적 버튼 활성화
    IEnumerator clearBtn()
    {
        yield return new WaitForSecondsRealtime(7f);

        homeBtn.SetActive(true);
        recordBtn.SetActive(true); 
        playAgainBtn.SetActive(true);
    }


}

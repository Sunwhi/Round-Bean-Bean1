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

    //Popper 관련
    Vector3 popperInitialScale = new Vector3(0, 0, 0);
    Vector3 popperTargetScale1 = new Vector3(4, 3, 3);
    Vector3 popperTargetScale2 = new Vector3(4, 4, 3);
    Vector3 popperMinScale1 = new Vector3(2, 1, 1);
    Vector3 popperMinScale2 = new Vector3(2, 2, 1);
    private float timeElapsed1 = 0f;
    private float timeElapsed2 = 0f;   
    private float popperAnimaionDuration = 0.0001f;
    private float timer2;

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

    IEnumerator PopperAnimation()
    {
        if(timeElapsed1 < popperAnimaionDuration)
        {
            timeElapsed1 += Time.deltaTime;
            float t = timeElapsed1 / popperAnimaionDuration;
            popperLeft.transform.localScale = Vector3.Lerp(popperInitialScale, popperTargetScale1, t);
            popperRight.transform.localScale = Vector3.Lerp(popperInitialScale, popperTargetScale2, t);
        }
        if(timeElapsed2 > popperAnimaionDuration)
        {
            timeElapsed2 += Time.deltaTime;
            float t = Mathf.PingPong(Time.time / popperAnimaionDuration, 1);
            popperLeft.transform.localScale = Vector3.Lerp(popperMinScale1, popperTargetScale1, t);
            popperRight.transform.localScale = Vector3.Lerp(popperMinScale2,popperTargetScale2, t);
        }    
        yield return null;
    }
    //클리어 시 8초 후(점수 크게 나타나지고 얼마후) 홈 버튼과 업적 버튼 활성화
    IEnumerator clearBtn()
    {
        yield return new WaitForSecondsRealtime(8f);

        homeBtn.SetActive(true);
        recordBtn.SetActive(true); 
        playAgainBtn.SetActive(true);
    }


}

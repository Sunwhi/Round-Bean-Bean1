using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ClearScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] GameObject newRecordText;
    [SerializeField] GameObject clearText;
    [SerializeField] Image clearBackgroundPanel;
    [SerializeField] GameObject homeBtn;
    [SerializeField] GameObject recordBtn;
    [SerializeField] GameObject playAgainBtn;

    private Vector2 startPosition;
    [SerializeField] Vector2 endPosition;
    private float animationDuration = 3f;
    private float startScale = 2f;
    private float endScale = 4f;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = finalScoreText.rectTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < animationDuration && GameManager.Instance.gameClear && !clearText.activeSelf)
        {
            // 시간 지남에 따라 스코어 커지면서 가운데로 오게
            timer += Time.deltaTime;
            
            finalScoreText.rectTransform.localPosition = Vector2.Lerp(startPosition, endPosition, timer / animationDuration);
             
            float scale = Mathf.Lerp(startScale,endScale,timer / animationDuration);
            finalScoreText.rectTransform.localScale = scale * Vector2.one;

            // 클리어 배경 화면 어둡게, panel이용해서 
            Color color = clearBackgroundPanel.color;
            color.a = Mathf.Lerp(0, 0.7f, timer / animationDuration);
            clearBackgroundPanel.color = color;

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
    //클리어 시 8초 후(점수 크게 나타나지고 얼마후) 홈 버튼과 업적 버튼 활성화
    IEnumerator clearBtn()
    {
        yield return new WaitForSecondsRealtime(8f);

        homeBtn.SetActive(true);
        recordBtn.SetActive(true); 
        playAgainBtn.SetActive(true);
    }


}

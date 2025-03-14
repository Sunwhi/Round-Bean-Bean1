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

    /*Popper ����*/
    //1�� ����, 2�� ������
    Vector3 popperZeroScale = new Vector3(0, 0, 0);
    //pingpong�� max�� minscale, popper�׸��� Ŀ���� �۾����� �Ҷ� ���� scale����
    Vector3 popperMaxScale1 = new Vector3(4, 3, 3);
    Vector3 popperMaxScale2 = new Vector3(4, 4, 3);
    Vector3 popperMinScale1 = new Vector3(3, 2, 1);
    Vector3 popperMinScale2 = new Vector3(2, 3, 1);
    //pingpong�� ���������� popper scalse
    Vector3 popperLastScale1;
    Vector3 popperLastScale2;

    private float timeElapsed1 = 0f;
    private float timeElapsed2 = 0f;   
    private float popperDisappearDuration = 0.6f; // popper�� ������� �ִϸ��̼��� �ӵ�
    private float popperPingPongDuration = 0.6f; // popper�� PingPong�ϴ� �ӵ�(Ŀ���� �۾������ϴ�)
    private float popperPingPongTime = 4.5f; // popper PingPong�� ���ӽð�(Ŀ���� �۾����� �ִϸ��̼��� ���� �ð�)

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
            // �ð� ������ ���� ���ھ� Ŀ���鼭 ����� ����
            timer += Time.deltaTime;
            
            finalScoreText.rectTransform.localPosition = Vector2.Lerp(startPosition, endPosition, timer / animationDuration);
             
            float scale = Mathf.Lerp(startScale,endScale,timer / animationDuration);
            finalScoreText.rectTransform.localScale = scale * Vector2.one;

            // Ŭ���� ��� ȭ�� ��Ӱ�, panel�̿��ؼ� 
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
                // �ű���̶�� NEW RECORD!!
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
        // popper�� Ŀ���� �۾����� �ϴ� �ִϸ��̼�
        if(timeElapsed1 < popperPingPongTime)
        {
            timeElapsed1 += Time.deltaTime;

            float t = Mathf.PingPong(Time.time / popperPingPongDuration, 1);

            popperLeft.transform.localScale = Vector3.Lerp(popperMaxScale1, popperMinScale1, t);
            popperRight.transform.localScale = Vector3.Lerp(popperMaxScale2, popperMinScale2, t);

            popperLastScale1 = popperLeft.transform.localScale;
            popperLastScale2 = popperRight.transform.localScale;
        }
        // popper�� �۾����鼭 ������� �ִϸ��̼�
        if(timeElapsed1 > popperPingPongTime && timeElapsed2 < popperDisappearDuration)
        {
            timeElapsed2 += Time.deltaTime;
            
            float t = timeElapsed2 / popperDisappearDuration;
            
            popperLeft.transform.localScale = Vector3.Lerp(popperLastScale1, popperZeroScale, t);
            popperRight.transform.localScale = Vector3.Lerp(popperLastScale2, popperZeroScale, t);
        }
        yield return null;
    }
    //Ŭ���� �� 8�� ��(���� ũ�� ��Ÿ������ ����) Ȩ ��ư�� ���� ��ư Ȱ��ȭ
    IEnumerator clearBtn()
    {
        yield return new WaitForSecondsRealtime(7f);

        homeBtn.SetActive(true);
        recordBtn.SetActive(true); 
        playAgainBtn.SetActive(true);
    }


}

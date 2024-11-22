using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClearScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] GameObject clearText;
    [SerializeField] Image clearBackgroundPanel;
    [SerializeField] GameObject homeBtn;
    [SerializeField] GameObject recordBtn;

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
            timer += Time.deltaTime;

            finalScoreText.rectTransform.localPosition = Vector2.Lerp(startPosition, endPosition, timer / animationDuration);
             
            float scale = Mathf.Lerp(startScale,endScale,timer / animationDuration);
            finalScoreText.rectTransform.localScale = scale * Vector2.one;

            Color color = clearBackgroundPanel.color;
            color.a = Mathf.Lerp(0, 0.7f, timer / animationDuration);
            clearBackgroundPanel.color = color;

            if(!homeBtn.activeSelf)
            {
                StartCoroutine(clearBtn());
            }
        }

    }
    IEnumerator clearBtn()
    {
        yield return new WaitForSecondsRealtime(8f);

        homeBtn.SetActive(true);
        recordBtn.SetActive(true); 
    }
}

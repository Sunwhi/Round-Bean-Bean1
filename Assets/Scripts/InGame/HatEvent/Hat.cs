using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hat : MonoBehaviour
{
    [SerializeField] Transform hatTargetPosition;
    [SerializeField] GameObject wheel;
    private UnicycleController unicycleController;
    private PlayerDragMovement playerDragMovement;
    Sequence sequence;
    // Start is called before the first frame update
    void Start()
    {
        //sequence = DOTween.Sequence();
        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent<PlayerDragMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HatAnimation();
        }
    }
    void HatAnimation()
    {
        unicycleController.enabled = false;
        playerDragMovement.enabled = false;
        Time.timeScale = 0;
        Debug.Log(Time.timeScale);
        transform.DOMove(hatTargetPosition.position, 1.0f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                unicycleController.enabled = true;
                playerDragMovement.enabled = true;
            });
    }
}

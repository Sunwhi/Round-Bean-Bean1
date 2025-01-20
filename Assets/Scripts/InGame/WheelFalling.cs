using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelFalling : MonoBehaviour
{
    public AudioClip playerFallClip;
    private int sfxOnce;
    // Start is called before the first frame update
    void Start()
    {
        sfxOnce = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -1.5)
        {
            if(sfxOnce == 0)
            {
                SoundManager.Instance.SFXPlay("PlayerFall", playerFallClip);
                sfxOnce = 1;
            }
            GameManager.Instance.gameOver = true;
        }
    }
}

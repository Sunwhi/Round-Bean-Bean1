using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteBypassSFX : MonoBehaviour
{
    /*
     * 효과음 재생하는 스크립트
     */
    public AudioClip clip; // Inspector에서 효과음 지정

    public void PlayEffectSound()
    {
        if (clip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.SFXPlay(gameObject.name, clip); // SoundManager의 SFXPlay 사용
        }
    }
}

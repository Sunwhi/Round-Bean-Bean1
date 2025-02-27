using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteBypassSFX : MonoBehaviour
{
    public AudioClip clip; // Inspector에서 효과음 지정

    public void PlayEffectSound()
    {
        if (clip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.SFXPlay(gameObject.name, clip); // SoundManager의 SFXPlay 사용
        }
    }
}

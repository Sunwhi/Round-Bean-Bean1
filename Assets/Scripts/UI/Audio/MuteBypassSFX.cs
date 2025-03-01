using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteBypassSFX : MonoBehaviour
{
    /*
     * ȿ���� ����ϴ� ��ũ��Ʈ
     */
    public AudioClip clip; // Inspector���� ȿ���� ����

    public void PlayEffectSound()
    {
        if (clip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.SFXPlay(gameObject.name, clip); // SoundManager�� SFXPlay ���
        }
    }
}

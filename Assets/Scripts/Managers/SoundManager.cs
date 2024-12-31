using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    private Sprite soundOnImage;
    public Sprite soundOffImage;
    public Button button;
    private bool isOn = true;

    public AudioSource bgSound;
    public AudioClip[] bglist;
    public GroundScroller groundScroller;
    public AudioSource audioSource;
    public static SoundManager instance;

    private int lastPlayedClipIndex = -1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            StartCoroutine(FadeInBGM(bglist[0]));
        }
        else if (scene.name == "InGame")
        {
            if (groundScroller == null)
            {
                groundScroller = FindObjectOfType<GroundScroller>();
                if (groundScroller == null)
                {
                    Debug.LogError("GroundScroller not found in InGame scene.");
                }
            }
            PlaySoundWithDistance(groundScroller != null ? groundScroller.distance : 0);
        }
    }

    void Start()
    {
        soundOnImage = button.image.sprite;
    }

    public void ButtonClicked()
    {
        isOn = !isOn;
        button.image.sprite = isOn ? soundOnImage : soundOffImage;
        bgSound.mute = !isOn;
    }

    void Update()
    {
        if (groundScroller != null && SceneManager.GetActiveScene().name == "InGame")
        {
            float distance = groundScroller.distance;
            PlaySoundWithDistance(distance);
        }
    }

    public void PlaySoundWithDistance(float distance)
    {
        int clipIndex = GetClipIndexByDistance(distance);
        if (clipIndex != lastPlayedClipIndex)
        {
            lastPlayedClipIndex = clipIndex;
            StartCoroutine(FadeInBGM(bglist[clipIndex]));
        }
    }

    private int GetClipIndexByDistance(float distance)
    {
        if (distance < 4) return 0; // spring
        else if (distance < 20) return 1; // summer
        else if (distance < 30) return 2; // fall
        else if (distance < 42) return 3; // winter
        else return 0; // spring2
    }

    private IEnumerator FadeOutBGM()
    {
        while (bgSound.volume > 0)
        {
            bgSound.volume -= Time.deltaTime / 1.0f; // FadeOut 시간 1초
            yield return null;
        }
        bgSound.Stop();
        bgSound.volume = 0.5f; // 기본 볼륨 복원
    }

    private IEnumerator FadeInBGM(AudioClip newClip)
    {
        yield return StartCoroutine(FadeOutBGM());
        bgSound.clip = newClip;
        bgSound.Play();
        bgSound.volume = 0;
        while (bgSound.volume < 0.5f)
        {
            bgSound.volume += Time.deltaTime / 1.0f; // FadeIn 시간 1초
            yield return null;
        }
    }

    public void BgSoundPlay(AudioClip clip)
    {
        StartCoroutine(FadeInBGM(clip));
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audiosource = go.AddComponent<AudioSource>();
        audiosource.clip = clip;
        audiosource.Play();
        Destroy(go, clip.length);
    }
}

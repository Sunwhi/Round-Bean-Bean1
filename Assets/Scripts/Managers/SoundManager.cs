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

            EnsureAudioSourceExists();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void EnsureAudioSourceExists()
    {
        if (bgSound == null)
        {
            GameObject bgSoundObject = new GameObject("BgSound");
            bgSound = bgSoundObject.AddComponent<AudioSource>();
            bgSound.volume = 0.5f;
            bgSound.loop = true;
            DontDestroyOnLoad(bgSoundObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 버튼을 씬에서 찾아서 button 변수에 할당
        button = GameObject.Find("SoundBtn")?.GetComponent<Button>();
        if (button != null)
        {
            soundOnImage = button.image.sprite;
            button.onClick.AddListener(ButtonClicked);  // 버튼 클릭 이벤트 연결
        }
        else
        {
            Transform canvasTransform = GameObject.Find("Canvas").transform;

            if (canvasTransform != null)
            {
                // OptionalPanel은 Canvas의 첫 번째 자식 (예시: 인덱스 0)
                Transform optionalPanel = canvasTransform.GetChild(15);

                if (optionalPanel != null)
                {
                    // SoundBtn은 OptionalPanel의 첫 번째 자식 (예시: 인덱스 0)
                    Transform soundBtnTransform = optionalPanel.GetChild(5);

                    if (soundBtnTransform != null)
                    {
                        button = soundBtnTransform.GetComponent<Button>();
                        soundOnImage = button.image.sprite;
                        button.onClick.AddListener(ButtonClicked);

                    }
                }
            }

            if (button != null)
            {
                Debug.Log("Sound Button successfully assigned using GetChild!");
                
            }
            else
            {
                Debug.LogError("Sound Button not found!");
            }
        }
        EnsureAudioSourceExists();
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
        // 초기화 과정에서 button이 할당되었는지 확인하고 이벤트를 연결합니다.
        if (button != null)
        {
            button.onClick.AddListener(ButtonClicked);  // 버튼 클릭 이벤트 연결
        }
    }

    public void ButtonClicked()
    {
        if (button != null)  // 버튼이 null인 경우를 방지
        {
            isOn = !isOn;
            button.image.sprite = isOn ? soundOnImage : soundOffImage;
            bgSound.mute = !isOn;
        }
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

            if (clipIndex == 0 && SceneManager.GetActiveScene().name == "InGame") 
            {
                PlayBGMImmediately(bglist[clipIndex]);
            }
            else
            {
                StartCoroutine(FadeInBGM(bglist[clipIndex]));
            }
        }
    }

    private int GetClipIndexByDistance(float distance)
    {
        if (distance < 4) return 1; // spring
        else if (distance < 20) return 2; // summer
        else if (distance < 30) return 3; // fall
        else if (distance < 42) return 4; // winter
        else return 5; // spring2
    }

    private IEnumerator FadeOutBGM()
    {
        while (bgSound.volume > 0)
        {
            bgSound.volume -= Time.deltaTime / 1.0f; // FadeOut
            yield return null;
        }
        bgSound.Stop();
        bgSound.volume = 0.5f; 
    }

    private IEnumerator FadeInBGM(AudioClip newClip)
    {
        yield return StartCoroutine(FadeOutBGM());
        bgSound.clip = newClip;
        bgSound.Play();
        bgSound.volume = 0;
        while (bgSound.volume < 0.5f)
        {
            bgSound.volume += Time.deltaTime / 1.0f; // FadeIn 
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

    private void PlayBGMImmediately(AudioClip newClip)
    {
        bgSound.Stop();           
        bgSound.clip = newClip;   
        bgSound.volume = 0.5f;    
        bgSound.Play();          
    }

}
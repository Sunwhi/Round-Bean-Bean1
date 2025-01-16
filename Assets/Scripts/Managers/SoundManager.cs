using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    //public Sprite soundOnImage;
    //public Sprite soundOffImage;
    //public Button button;
    //public Button offbutton;
    //private bool isOn = true;

    public AudioSource bgSound;
    public AudioClip[] bglist;
    public GroundScroller groundScroller;
    public AudioSource audioSource;
    private List<AudioSource> audioSources = new List<AudioSource>(); 

    private int lastPlayedClipIndex = -1;


    public static SoundManager Instance { get; private set; } // 싱글톤 인스턴스
    private void Awake()
    {
        EnsureAudioSourceExists();
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때만 이벤트 처리

        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지 -> 씬 재시작(게임 재시작)시에 변수들이 초기화되지 않는 문제가 발생! DontDestroyOnLoad는 잠시 끄는걸로
        }
        else
        {
            Destroy(gameObject); // 이미 GameManager가 존재하면 새로 생성된 오브젝트는 삭제
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 씬 전환 시 이벤트 제거
    }
    private void EnsureAudioSourceExists()
    {
        if (bgSound == null)
        {
            bgSound = gameObject.AddComponent<AudioSource>();
            bgSound.volume = 0.5f;
            bgSound.loop = true;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 버튼을 씬에서 찾아서 button 변수에 할당
        //button = GameObject.Find("SoundBtn")?.GetComponent<Button>();
        //if (button != null)
        //{
        //    Transform canvasTransform = GameObject.Find("MainCanvas").transform;
        //    if (canvasTransform != null)
        //    {
        //        // OptionalPanel은 Canvas의 첫 번째 자식 (예시: 인덱스 0)
        //        Transform optionalPanel = canvasTransform.GetChild(5);
        //        if (optionalPanel != null)
        //        {
        //            offbutton = optionalPanel.GetComponent<Button>();
        //            soundOffImage = offbutton.image.sprite;
        //        }
        //    }
        //    soundOnImage = button.image.sprite;
        //    button.onClick.AddListener(ButtonClicked);  // 버튼 클릭 이벤트 연결
        //}
        //else
        //{
        //    Transform canvasTransform = GameObject.Find("Canvas").transform;
        //
        //    if (canvasTransform != null)
        //    {
        //        // OptionalPanel은 Canvas의 첫 번째 자식 (예시: 인덱스 0)
        //        Transform optionalPanel = canvasTransform.GetChild(15);
        //
        //        if (optionalPanel != null)
        //        {
        //            // SoundBtn은 OptionalPanel의 첫 번째 자식 (예시: 인덱스 0)
        //            Transform soundBtnTransform = optionalPanel.GetChild(4);
        //            Transform soundBtnTransform2 = optionalPanel.GetChild(5);
        //
        //            if (soundBtnTransform != null)
        //            {
        //                offbutton= soundBtnTransform2.GetComponent<Button>();
        //                button = soundBtnTransform.GetComponent<Button>();
        //                soundOffImage = offbutton.image.sprite;
        //                soundOnImage = button.image.sprite;
        //                button.onClick.AddListener(ButtonClicked);
        //            }
        //        }
        //    }
        //}
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
        //if (button != null)
        //{
        //    button.onClick.AddListener(ButtonClicked);  // 버튼 클릭 이벤트 연결
        //}
    }

   //public void ButtonClicked()
   //{
   //    if (button != null)  // 버튼이 null인 경우를 방지
   //    {
   //        isOn = !isOn;
   //        button.image.sprite = isOn ? soundOnImage : soundOffImage;
   //        bgSound.mute = !isOn;
   //    }
   //}

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
        if (distance < 15) return 1; // spring
        else if (distance < 30) return 2; // summer
        else if (distance < 40) return 3; // fall
        else if (distance < 55) return 4; // winter
        else return 5; // spring2
    }

    private IEnumerator FadeOutBGM()
    {
        while (bgSound.volume > 0)
        {
            bgSound.volume -= Time.deltaTime / 2.5f; // FadeOut
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
            bgSound.volume += Time.deltaTime / 2.5f; // FadeIn 
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
    public void RegisterAudioSource(AudioSource audioSource)
    {
        if (!audioSources.Contains(audioSource))
        {
            audioSources.Add(audioSource);
        }
    }

    public void StopAllSounds()
    {
        foreach (var audioSource in audioSources)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }

    public void ResumeAllSounds()
    {
        foreach (var audioSource in audioSources)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

}
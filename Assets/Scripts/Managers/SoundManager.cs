using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgSound;
    public AudioClip[] bglist;
    public GroundScroller groundScroller;
    private List<AudioSource> audioSources = new List<AudioSource>();

    private int lastPlayedClipIndex = -1;
    private bool isMuted = false; // 음소거 상태 변수
    public Button muteButton; // 음소거 버튼
    public Sprite soundOnImage; // 음소거 해제 아이콘
    public Sprite soundOffImage; // 음소거 아이콘
    public Sprite soundOnImageMainScene; // 메인 씬용 음소거 해제 아이콘
    public Sprite soundOffImageMainScene; // 메인 씬용 음소거 아이콘

    public static SoundManager Instance { get; private set; } // 싱글톤 인스턴스

    private void Awake()
    {
        EnsureAudioSourceExists();
        SceneManager.sceneLoaded += OnSceneLoaded; // 씬이 로드될 때만 이벤트 처리

        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 GameManager가 존재하면 새로 생성된 오브젝트는 삭제
        }

        // 음소거 상태 불러오기
        isMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1; // 1이면 음소거, 0이면 음소거 아님
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
            bgSound.spatialBlend = 0;        // 2D 사운드로 설정
            bgSound.priority = 128;          // 우선순위 낮춤 (0이 가장 높음)
            bgSound.dopplerLevel = 0;        // Doppler 효과 비활성화
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureAudioSourceExists();

        // 음소거 버튼 이벤트 연결
        if (muteButton != null)
        {
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(ToggleMute); // 버튼 클릭 시 음소거 토글
        }

        if (scene.name == "MainScene")
        {
            StartCoroutine(FadeInBGM(bglist[0]));
            // 메인 씬에서는 다른 아이콘 사용
            muteButton.image.sprite = isMuted ? soundOffImageMainScene : soundOnImageMainScene;
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
            // 인게임 씬에서는 다른 아이콘 사용
            muteButton.image.sprite = isMuted ? soundOffImage : soundOnImage;
        }

        // 음소거 상태 적용
        bgSound.mute = isMuted;
    }

    private void Start()
    {
        foreach (var clip in bglist)
        {
            if (clip.loadState != AudioDataLoadState.Loaded)
            {
                clip.LoadAudioData();
            }
        }
    }
    public GameObject specificPanel; 

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "InGame")
        {
            float distance = groundScroller != null ? groundScroller.distance : 0;
            PlaySoundWithDistance(distance);

            // 특정 패널이 활성화되어 있다면 배경 음악 정지
            if (specificPanel != null && specificPanel.activeSelf)
            {
                if (bgSound.isPlaying)
                {
                    bgSound.Pause();
                }
            }
            else
            {
                if (!bgSound.isPlaying && !isMuted)
                {
                    bgSound.Play();
                }
            }
        }
    }

    public void ToggleMute()
    {
        isMuted = !isMuted; // 음소거 상태 반전
        bgSound.mute = isMuted; // 음소거 적용

        // 음소거 상태 저장
        PlayerPrefs.SetInt("SoundMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        // 버튼 아이콘 업데이트
        UpdateMuteButtonIcon();
    }

    private void UpdateMuteButtonIcon()
    {
        if (muteButton != null)
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                muteButton.image.sprite = isMuted ? soundOffImageMainScene : soundOnImageMainScene; // 메인 씬 아이콘
            }
            else
            {
                muteButton.image.sprite = isMuted ? soundOffImage : soundOnImage; // 인게임 씬 아이콘
            }
        }
    }

    private Coroutine currentFadeCoroutine = null;

    public void PlaySoundWithDistance(float distance)
    {
        int clipIndex = GetClipIndexByDistance(distance);
        if (clipIndex != lastPlayedClipIndex)
        {
            lastPlayedClipIndex = clipIndex;

            if (currentFadeCoroutine != null)
            {
                StopCoroutine(currentFadeCoroutine);
            }
            currentFadeCoroutine = StartCoroutine(FadeInBGM(bglist[clipIndex]));
        }
    }

    private int GetClipIndexByDistance(float distance)
    {
        if (distance < 50 - 10) return 1; // spring
        else if (distance < 100 - 10) return 2; // summer
        else if (distance < 155 - 10) return 3; // fall
        else if (distance < 215 - 10) return 4; // winter
        else return 5; // spring2
    }

    private IEnumerator FadeOutBGM()
    {
        while (bgSound.volume > 0)
        {
            bgSound.volume -= Time.deltaTime / 2.5f;
            yield return null;
        }
        bgSound.Stop();
        bgSound.volume = 0.5f;
    }

    private IEnumerator FadeInBGM(AudioClip newClip)
    {
        if (bgSound.isPlaying)
        {
            float startVolume = bgSound.volume;
            for (float t = 0; t < 1; t += Time.deltaTime / 1.5f)
            {
                bgSound.volume = Mathf.Lerp(startVolume, 0, t);
                yield return null;
            }
            bgSound.Stop();
        }

        bgSound.clip = newClip;
        bgSound.Play();

        for (float t = 0; t < 1; t += Time.deltaTime / 1.5f)
        {
            bgSound.volume = Mathf.Lerp(0, 0.5f, t);
            yield return null;
        }
        bgSound.volume = 0.5f;
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

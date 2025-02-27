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
    private bool isMuted = false; // ���Ұ� ���� ����
    public Button muteButton; // ���Ұ� ��ư
    public Sprite soundOnImage; // ���Ұ� ���� ������
    public Sprite soundOffImage; // ���Ұ� ������
    public Sprite soundOnImageMainScene; // ���� ���� ���Ұ� ���� ������
    public Sprite soundOffImageMainScene; // ���� ���� ���Ұ� ������

    public static SoundManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    private void Awake()
    {
        EnsureAudioSourceExists();
        SceneManager.sceneLoaded += OnSceneLoaded; // ���� �ε�� ���� �̺�Ʈ ó��

        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �̹� GameManager�� �����ϸ� ���� ������ ������Ʈ�� ����
        }

        // ���Ұ� ���� �ҷ�����
        isMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1; // 1�̸� ���Ұ�, 0�̸� ���Ұ� �ƴ�
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �� ��ȯ �� �̺�Ʈ ����
    }

    private void EnsureAudioSourceExists()
    {
        if (bgSound == null)
        {
            bgSound = gameObject.AddComponent<AudioSource>();
            bgSound.volume = 0.5f;
            bgSound.loop = true;
            bgSound.spatialBlend = 0;        // 2D ����� ����
            bgSound.priority = 128;          // �켱���� ���� (0�� ���� ����)
            bgSound.dopplerLevel = 0;        // Doppler ȿ�� ��Ȱ��ȭ
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureAudioSourceExists();

        // ���Ұ� ��ư �̺�Ʈ ����
        if (muteButton != null)
        {
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(ToggleMute); // ��ư Ŭ�� �� ���Ұ� ���
        }

        if (scene.name == "MainScene")
        {
            StartCoroutine(FadeInBGM(bglist[0]));
            // ���� �������� �ٸ� ������ ���
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
            // �ΰ��� �������� �ٸ� ������ ���
            muteButton.image.sprite = isMuted ? soundOffImage : soundOnImage;
        }

        // ���Ұ� ���� ����
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

            // Ư�� �г��� Ȱ��ȭ�Ǿ� �ִٸ� ��� ���� ����
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
        isMuted = !isMuted; // ���Ұ� ���� ����
        bgSound.mute = isMuted; // ���Ұ� ����

        // ���Ұ� ���� ����
        PlayerPrefs.SetInt("SoundMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();

        // ��ư ������ ������Ʈ
        UpdateMuteButtonIcon();
    }

    private void UpdateMuteButtonIcon()
    {
        if (muteButton != null)
        {
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                muteButton.image.sprite = isMuted ? soundOffImageMainScene : soundOnImageMainScene; // ���� �� ������
            }
            else
            {
                muteButton.image.sprite = isMuted ? soundOffImage : soundOnImage; // �ΰ��� �� ������
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

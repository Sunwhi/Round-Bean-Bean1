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


    public static SoundManager Instance { get; private set; } // �̱��� �ν��Ͻ�
    private void Awake()
    {
        EnsureAudioSourceExists();
        SceneManager.sceneLoaded += OnSceneLoaded; // ���� �ε�� ���� �̺�Ʈ ó��

        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // ���� �ٲ� ���� -> �� �����(���� �����)�ÿ� �������� �ʱ�ȭ���� �ʴ� ������ �߻�! DontDestroyOnLoad�� ��� ���°ɷ�
        }
        else
        {
            Destroy(gameObject); // �̹� GameManager�� �����ϸ� ���� ������ ������Ʈ�� ����
        }
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
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ��ư�� ������ ã�Ƽ� button ������ �Ҵ�
        //button = GameObject.Find("SoundBtn")?.GetComponent<Button>();
        //if (button != null)
        //{
        //    Transform canvasTransform = GameObject.Find("MainCanvas").transform;
        //    if (canvasTransform != null)
        //    {
        //        // OptionalPanel�� Canvas�� ù ��° �ڽ� (����: �ε��� 0)
        //        Transform optionalPanel = canvasTransform.GetChild(5);
        //        if (optionalPanel != null)
        //        {
        //            offbutton = optionalPanel.GetComponent<Button>();
        //            soundOffImage = offbutton.image.sprite;
        //        }
        //    }
        //    soundOnImage = button.image.sprite;
        //    button.onClick.AddListener(ButtonClicked);  // ��ư Ŭ�� �̺�Ʈ ����
        //}
        //else
        //{
        //    Transform canvasTransform = GameObject.Find("Canvas").transform;
        //
        //    if (canvasTransform != null)
        //    {
        //        // OptionalPanel�� Canvas�� ù ��° �ڽ� (����: �ε��� 0)
        //        Transform optionalPanel = canvasTransform.GetChild(15);
        //
        //        if (optionalPanel != null)
        //        {
        //            // SoundBtn�� OptionalPanel�� ù ��° �ڽ� (����: �ε��� 0)
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
        // �ʱ�ȭ �������� button�� �Ҵ�Ǿ����� Ȯ���ϰ� �̺�Ʈ�� �����մϴ�.
        //if (button != null)
        //{
        //    button.onClick.AddListener(ButtonClicked);  // ��ư Ŭ�� �̺�Ʈ ����
        //}
    }

   //public void ButtonClicked()
   //{
   //    if (button != null)  // ��ư�� null�� ��츦 ����
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
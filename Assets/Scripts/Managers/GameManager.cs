using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Properties;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
/*
 * GameManager Ŭ���� 
 * �̱��� �̿��Ͽ� �ٸ� ���̳� �ٸ� ������Ʈ���� GameManager Ŭ���� �� �Լ��� ���� ���� ����!
 */
public class GameManager : MonoBehaviour
{
    Scene inGameScene; // ���� �ΰ��� �� ����

    [SerializeField] Collider2D animalCollider;
    public bool gameOver = false;
    public bool gameClear = false;
    public bool gamePaused = false; // ���� �ߴ� -> ���� ���� ��, ����â
    int callFinalScoreOnce; // ���� ������ �� �ѹ��� ��� �����ϱ� ���� ���� �ӽú���
    int callReplaceJointOnce;
    // GameOver(), GameClear() �Լ� �� �ѹ����� ȣ���ϱ� ���� ����
    //bool isGameOverCalled = false; 
    //bool isGameClearCalled = false;

    float time; // ���� �ð� ���
    public float finalScore; // ���� �ð� ���
    public bool isNewRecord = false; // NEW RECORD! ��� ���� ����
    [SerializeField] TextMeshProUGUI textTime;  // �ð��� ��Ÿ���� �ؽ�Ʈ
    [SerializeField] GameObject textGameOver; // ���ӿ��� �ؽ�Ʈ, setActive() ȣ�� ���ؼ� GameObject�� 
    [SerializeField] GameObject textGameClear; // ���� Ŭ���� �ؽ�Ʈ
    [SerializeField] GameObject playAgainBtn;
    [SerializeField] GameObject particles1, particles2;

    [SerializeField] GameObject animal;
    [SerializeField] Rigidbody2D frameRigidBody;
    [SerializeField] GameObject wheel;
    [SerializeField] GameObject frame;
    private UnicycleController unicycleController;
    private PlayerDragMovement playerDragMovement;
    int jumpTwice = 0;

    public bool newHatGenerated = false;
    // ���� ����Ʈ���� ��
    [SerializeField] GameObject hat;
    SpriteRenderer hatSP;
    public float fadeDuration = 5f;
    public bool hatFall = false;
    public bool hatOn = false;
    public bool initializeHatOnce = true;

    // k->keyboard(UnicycleController.cs), t->touch(PlayerDragMovement.cs)
    // UnicycleController�� �־ �ɵ�?
    public bool kJumpWithNoHat = false; // ���ڰ� ���� ���¿��� �����ؼ� ���ڸ� �Ծ��� �� ������ ���� ���� ����. �ڼ��� ���� -> UnicycleController.cs
    public bool tJumpWithNoHat = false;
    private float timer;

    public static GameManager Instance { get; private set; } // �̱��� �ν��Ͻ�

    void Awake()
    {
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

    void Start()
    {
        inGameScene = SceneManager.GetActiveScene(); // ���� �۵��ǰ� �ִ� InGame���� ������

        unicycleController = wheel.GetComponent<UnicycleController>();
        playerDragMovement = wheel.GetComponent <PlayerDragMovement>();
        time = 0; // �� �Ǹ��� �ð� 0���� �ʱ�ȭ
        callFinalScoreOnce = 0; // �� �Ǹ��� 0���� �ʱ�ȭ

    }

    void Update()
    {
        //������ �ߴܵ��� �ʴ� �̻� �ð����� ����.
        if(!gamePaused && !hatOn) time += Time.deltaTime;
        //�����̺�Ʈ -> 1.5�� ������ ����
        if (hatOn) time += Time.deltaTime * 0.5f;

        //textTime�� ���� �ð��� ǥ��
        if (!gameOver && !gameClear)
            textTime.text = FormatTime(time);

        // AnimalCollision.cs���� isGround�� true�� �Ǿ��ٸ� GameOver�Լ��� ȣ���Ѵ�.
        // isGameOverCalled�� GameOver()�Լ��� �ѹ��� ����ǵ��� ��. �ƴϸ� ��� ȣ���. -> ��� ���
        if (gameOver)
        {
            GameOver();
        }
        // isGameClearCalled�� GameClear()�Լ��� �ѹ��� ����ǵ��� ��. �ƴϸ� ��� ȣ���.
        if (gameClear)
        {
            GameClear();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }

        //Find�� GetComponent�� �ѹ��� ȣ���ϱ� ����(������Ʈ �Ҵ�) initializeHatOnce ������ ����Ͽ� Upadte�Լ� ������ ȣ��
        if (hatFall && initializeHatOnce)
        {
            hat = GameObject.Find("Hat(Clone)");
            hatSP = hat.GetComponent<SpriteRenderer>(); // HatFall()�� ���Ǵ� ��������Ʈ ������ �ʱ�ȭ
            initializeHatOnce = false;
        }
        if (hatFall && !hat.IsDestroyed())
        {
            HatFall();
        }
    }

    private void HatFall()
    {
        //���ڿ� ĳ���� �浹����
        BoxCollider2D hatCollider = hat.GetComponent <BoxCollider2D>();
        CircleCollider2D wheelCollider = wheel.GetComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(hatCollider, wheelCollider); // 
        hat.layer = 3; // ���̻� ĳ���Ϳ� �ε����� �ʰ� ���̾ �ٲ۴�.

        //���ڰ� �Ӹ����� �������� ��
        //������ ������ ���߰� �ش� ���ڸ� ���ش�.
        timer += Time.deltaTime;
        Color color = hatSP.color;
        color.a = Mathf.Lerp(color.a, 0, timer / fadeDuration);
        hatSP.color = color;

        if (color.a < 0.01) // ���� �������
        {
            // ���� ���� �����Ǿ��� ���� ������ ���� �ʱ�ȭ
            initializeHatOnce = true; 
            hatFall = false;

            Destroy(hat);
        }
    }

    /* �� ������ time�� �а� �ʷ� ����� ǥ����
     * 00:00 (��, ��)�� ǥ���.
    */
    public string FormatTime(float time)
    {
        int seconds = Mathf.FloorToInt(time);
        int minutes = seconds / 60;
        int remainSeconds = seconds % 60;
        return $"{minutes:00} : {remainSeconds:00}";
    }

    /*
     * ���ӿ����� �������� �ۿ� ������ �Լ�
     */
    private void GameOver()
    {
        // ������ �� ������ �� �ѹ��� finalScore�� ���� / �� �ѹ��� PlayerPrefs ��� ���Ӱ� ����
        // �̰� ���ϸ� Update�� ��� ȣ��Ǿ� ��� ��ϵ��� ������.
        // ���ӿ��� ����� �ܼ� �׽�Ʈ ����. ���� ��ö� �ʿ����.
        /*if(callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            callFinalScoreOnce = 1;
        }*/

        textGameOver.SetActive(true); // ���ӿ��� �ؽ�Ʈ

        /* ���� �����ſ��� �и���Ű��, �̵��������� �������� ������ */
        animal.transform.SetParent(null);
        if(animal.GetComponent<Rigidbody2D>() == null)
        {
            animal.AddComponent<Rigidbody2D>();
            Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();

            if (frameRigidBody.angularVelocity > 0)
            {
                animalRigidBody.AddTorque(20f * Time.deltaTime);
            }
            else
            {
                animalRigidBody.AddTorque(-20f * Time.deltaTime);
            }
        }

        // �̵����� ��ũ��Ʈ ���Ƽ� ���ӿ��� ���� �������� ���ϰ� ��
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;

        
        // 'R' ���� ���� ����� -> ���� ���� �ٽ� �ε��Ѵ�.
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(inGameScene.name);
        }
        if(!playAgainBtn.activeSelf)
        {
            playAgainBtn.SetActive(true);
        }
    }

    private void GameClear()
    {
        // ������ �� ������ �� �ѹ��� finalScore�� ���� / �� �ѹ��� PlayerPrefs ��� ���Ӱ� ����
        // �̰� ���ϸ� Update�� ��� ȣ��Ǿ� ��� ��ϵ��� ������.
        if (callFinalScoreOnce == 0)
        {
            finalScore = time;
            RecordNewScore(finalScore);

            textGameClear.SetActive(true); // ���� Ŭ���� �ؽ�Ʈ
            particles1.SetActive(true);
            particles2.SetActive(true);

            callFinalScoreOnce = 1;
        }

        // �̵����� ��ũ��Ʈ ���Ƽ� ���ӿ��� ���� �������� ���ϰ� ��
        playerDragMovement.enabled = false;
        unicycleController.enabled = false;


        frameRigidBody.freezeRotation = true;

        // ���� ������ ���Ϸ� ������ ��ȯ
        float currentZRotation = frameRigidBody.transform.rotation.eulerAngles.z;
        if (currentZRotation > 180f) currentZRotation -= 360;

        // �����Ÿ� ������ �����.
        if (Mathf.Abs(currentZRotation) > 0.1)
        {
            Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
            float rotationSpeed = 0.05f;
            if(currentZRotation < 0)
            {
                frameRigidBody.transform.rotation = Quaternion.RotateTowards(frameRigidBody.transform.rotation, targetRotation, rotationSpeed);
            }
            else
            {
                frameRigidBody.transform.rotation = Quaternion.RotateTowards(frameRigidBody.transform.rotation, targetRotation, rotationSpeed);
            }
        }
        else
        {
            // �ӵ��� �پ��� animal ����
            if(frameRigidBody.velocity.magnitude < 0.1)
            {
                // ���� �ѹ�
                if (animal.GetComponent<Rigidbody2D>() == null)
                {
                    textGameClear.SetActive(false);
                    animal.transform.SetParent(null); // �ܹ������� ������ �����ҰŴϱ� �и�

                    animal.AddComponent<Rigidbody2D>();
                    Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();

                    animalRigidBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
                    jumpTwice++;
                }
                // ���� �ι�, ����
                else if(jumpTwice == 1)
                {
                    StartCoroutine(clearJump1());
                    StartCoroutine(clearJump2());
                    jumpTwice++;
                }
            }
        }


        // 'R' ���� ���� ����� -> ���� ���� �ٽ� �ε��Ѵ�.
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(inGameScene.name);
        }
    }
    // �ڷ�ƾ���� ������ ����. ù ��°, �� ��° ������ �ð����� �ΰ� �ڴ�.
    IEnumerator clearJump1()
    {
        yield return new WaitForSecondsRealtime(1.1f);
        Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();
        animalRigidBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
    IEnumerator clearJump2()
    {
        yield return new WaitForSecondsRealtime(2.2f);
        Rigidbody2D animalRigidBody = animal.GetComponent<Rigidbody2D>();
        animalRigidBody.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }
    /*
     * hingeJoint2D -> fixedJoint2D
     
    private void ReplaceJoint()
    {
        HingeJoint2D hingeJoint2D = frame.transform.GetComponent<HingeJoint2D>();

        if(hingeJoint2D != null)
        {
            Destroy(hingeJoint2D);
        }

        FixedJoint2D fixedJoint2D = frame.transform.AddComponent<FixedJoint2D>();
        fixedJoint2D.connectedBody = wheel.GetComponent<Rigidbody2D>();
    }*/
    /*
     * ���ھ ������ PlayerPrefs�� ���� ���ھ���� ������ ���� ���Ӱ� ����Ѵ�. 
     * ��, ���ĵǾ� ��ϵ��� ����.
     * ���߿� Ŭ���� ������ �� ���!
     */
    private void RecordNewScore(float finalScore)
    {
        int newScoreIdx = 0; // ���ο� ����� ����� Index
        string scoreNum; // PlayerPrefs�� ������ string
        string frontScoreNum; // ���ο� ��� ���������� scoreNum�� �� ����Ű��

        string dateNum;
        string frontDateNum;

        // ���ο� ����� ���° ����ΰ� ã��
        for(int i=0; i<10; i++)
        {
            scoreNum = "score" + i;

            if(finalScore < PlayerPrefs.GetFloat(scoreNum))
            { 
                newScoreIdx = i;
                break;
            }
            if (i == 4) return; // 5���� ��ϵ鿡 ������ ������ �ƿ� ��� X �׸��� return
        }

        if(newScoreIdx == 0) isNewRecord = true;
        // ���ο� ��� PlayerPrefs�� newScoreIdx�� �����ֱ�
        // �ڿ������� ������. �ڿ������� ��ĭ ���� ��ϵ��� �ڷ� �̵�
        for(int i = 9; i>=newScoreIdx; i--)
        {
            scoreNum = "score" + i;
            dateNum = "date" + i;

            if (i == newScoreIdx) // ���� ���Ӱ� ��ϵ� index��� ����ϰ� Ƣ��
            {
                PlayerPrefs.SetFloat(scoreNum, finalScore);
                PlayerPrefs.SetString(dateNum, DateTime.Now.ToString("yyyy-MM-dd"));
                return;
            }

            frontScoreNum = "score" + (i - 1);
            frontDateNum = "date" + (i - 1);    

            PlayerPrefs.SetFloat(scoreNum, PlayerPrefs.GetFloat(frontScoreNum)); // ���� score�� �ڷ� �ѱ��
            PlayerPrefs.SetString(dateNum, PlayerPrefs.GetString(frontDateNum));
        }
    }

}

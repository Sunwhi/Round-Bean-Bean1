using UnityEngine;

public class TouchObjectManager : MonoBehaviour
{
    public GameObject[] objectsToDelete; // 삭제할 오브젝트 리스트
    public GameObject[] objectsToActivate; // 활성화할 오브젝트 리스트

    void Update()
    {
        // Space 키를 눌렀을 때 삭제 동작 이건 테스트용
        if (Input.GetMouseButtonDown(0))
        {
            DeleteSpecifiedObjects();
            ActivateSpecifiedObjects();
        }

        // 터치가 발생했을 때 활성화 동작
        if (Input.touchCount > 0) // 화면에 하나 이상의 터치가 있을 경우
        {
            //DeleteSpecifiedObjects();
            //ActivateSpecifiedObjects();
        }
    }

    void DeleteSpecifiedObjects()
    {
        foreach (GameObject obj in objectsToDelete)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }

    void ActivateSpecifiedObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true); // 오브젝트 활성화
            }
        }
    }
}

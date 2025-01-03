using UnityEngine;

public class TouchObjectManager : MonoBehaviour
{
    public GameObject[] objectsToDelete; // ������ ������Ʈ ����Ʈ
    public GameObject[] objectsToActivate; // Ȱ��ȭ�� ������Ʈ ����Ʈ

    void Update()
    {
        // Space Ű�� ������ �� ���� ���� �̰� �׽�Ʈ��
        if (Input.GetMouseButtonDown(0))
        {
            DeleteSpecifiedObjects();
            ActivateSpecifiedObjects();
        }

        // ��ġ�� �߻����� �� Ȱ��ȭ ����
        if (Input.touchCount > 0) // ȭ�鿡 �ϳ� �̻��� ��ġ�� ���� ���
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
                obj.SetActive(true); // ������Ʈ Ȱ��ȭ
            }
        }
    }
}

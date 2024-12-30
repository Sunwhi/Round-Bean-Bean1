using UnityEngine;

public class TouchObjectDeleter : MonoBehaviour
{
    public GameObject[] objectsToDelete;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DeleteSpecifiedObjects();
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
}

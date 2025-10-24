using UnityEngine;

public class PresistentPlayer : MonoBehaviour
{
    private static PresistentPlayer instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

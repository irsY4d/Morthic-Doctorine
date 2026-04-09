using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentRoot : MonoBehaviour
{
    public static PersistentRoot instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int index = scene.buildIndex;

        // Scene 0 dan Scene >= 4 → hancurkan
        if (index < 1 || index > 3)
        {
            Destroy(gameObject);
            instance = null;
        }
    }

    public void ResetAll()
    {
        Destroy(gameObject);
        instance = null;
    }
}

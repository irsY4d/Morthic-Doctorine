using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionManager
{
    public static string LastEntryID { get; private set; }

    public static void LoadScene(string sceneName, string entryID)
    {
        LastEntryID = entryID;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Unload semua scene selain Persistent
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "C_PersistentScene")
            {
                SceneManager.UnloadSceneAsync(scene);   
            }
        }

        // Load scene baru secara additive
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found in Persistent scene!");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            return;
        }

        var spawnPoints = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Exclude);
        foreach (var sp in spawnPoints)
        {
            if (sp.entryID == LastEntryID)
            {
                player.transform.position = sp.transform.position;
                break;
            }
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionManager
{
    public static string LastEntryID { get; private set; }

    public static void LoadScene(string sceneName, string entryID)
    {
        LastEntryID = entryID;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene! (is it persistent?)");
            SceneManager.sceneLoaded -= OnSceneLoaded;
            return;
        }

        var spawnPoints = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsSortMode.None);
        foreach (var sp in spawnPoints)
        {
            if (sp.entryID == LastEntryID)
            {
                player.transform.position = sp.transform.position;
                break;
            }
        }

        // Lepas listener agar tidak dobel di scene berikutnya
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

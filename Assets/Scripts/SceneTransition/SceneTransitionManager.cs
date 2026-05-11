using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionManager
{
    public static string LastEntryID { get; private set; }
    public static string LastCheckpointID { get; private set; }
    public static bool IsRespawning { get; private set; }

    public static void LoadScene(string sceneName, string entryID, bool isRespawn = false)
    {
        LastEntryID = entryID;
        IsRespawning = isRespawn;

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

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { SceneManager.sceneLoaded -= OnSceneLoaded; return; }

        if (IsRespawning)
        {
            // Jika sedang RESPawn, cari objek RespawnChekpoint
            var checkpoints = Object.FindObjectsByType<RespawnChekpoint>(FindObjectsInactive.Exclude);
            foreach (var cp in checkpoints)
            {
                if (cp.respawnPointID == LastEntryID)
                {
                    player.transform.position = cp.transform.position;
                    break;
                }
            }
        }
        else
        {
            var spawnPoints = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Exclude);
            bool found = false;

            Debug.Log($"[DEBUG] Scene 2 dimuat. Jumlah SpawnPoint tersedia: {spawnPoints.Length}");
            Debug.Log($"[DEBUG] Mencari EntryID: '{LastEntryID}'");

            foreach (var sp in spawnPoints)
            {
                if (sp.entryID == LastEntryID)
                {
                    // TITIK PEMBUKTIAN
                    Vector3 targetPos = sp.transform.position;
                    player.transform.position = targetPos;

                    found = true;
                    Debug.Log($"[DEBUG] TRANSFORM BERHASIL! Player pindah ke {sp.name} di {targetPos}");
                    break;
                }
            }

            if (!found)
            {
                // TITIK MASALAH
                Debug.LogError($"[DEBUG] TRANSFORM GAGAL! Tidak ada SpawnPoint dengan ID '{LastEntryID}' di Scene 2.");
            }
        }

        IsRespawning = false; // Reset flag
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void ResetData()
    {
        LastEntryID = null;
        IsRespawning = false;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnChekpoint : MonoBehaviour
{
    public string respawnPointID;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.CompareTag("Player"))
            {
                Scene activeScene = SceneManager.GetActiveScene();

                // JANGAN simpan checkpoint jika scene yang aktif terdeteksi sebagai persistent
                if (activeScene.name == "C_PersistentScene")
                {
                    // Cari scene gameplay yang sedang loaded
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        Scene s = SceneManager.GetSceneAt(i);
                        if (s.name != "C_PersistentScene")
                        {
                            activeScene = s;
                            break;
                        }
                    }
                }

                GameManager.Instance.lastCheckpointScene = activeScene.name;
                GameManager.Instance.lastCheckpointID = respawnPointID;

                Debug.Log($"Checkpoint Berhasil Set: {respawnPointID} di {activeScene.name}");
            }
        }
    }
}

using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class ConfinerUpdater : MonoBehaviour
{
    private CinemachineCamera cineCam;
    private CinemachineConfiner2D confiner;

    void Start()
    {
        SetupCinemachine();

        // Register supaya setiap kali scene berubah, kamera update lagi
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupCinemachine();
    }

    private void SetupCinemachine()
    {
        // Cari kamera virtual Cinemachine
        if (cineCam == null)
            cineCam = FindFirstObjectByType<CinemachineCamera>();

        if (cineCam == null)
        {
            Debug.LogWarning("❌ CinemachineCamera tidak ditemukan di scene!");
            return;
        }

        // Update target follow/lookAt (player)
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            cineCam.Follow = player.transform;
            cineCam.LookAt = player.transform;
        }
        else
        {
            Debug.LogWarning("⚠️ Player belum ada di scene saat kamera setup.");
        }

        // Update confiner border
        if (confiner == null)
            confiner = cineCam.GetComponent<CinemachineConfiner2D>();

        if (confiner != null)
        {
            confiner.BoundingShape2D = GetComponent<Collider2D>();
            confiner.InvalidateBoundingShapeCache();
        }

        Debug.Log($"✅ Kamera diatur ulang untuk scene: {SceneManager.GetActiveScene().name}");
    }
}

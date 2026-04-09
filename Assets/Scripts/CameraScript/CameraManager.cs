using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CinemachineCamera[] cineCam;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        SetupCinemachine();
    }


    private void SetupCinemachine()
    {
        // Update target follow/lookAt (player)
        var player = GameObject.FindGameObjectWithTag("Player");
        
        foreach (var cam in cineCam)
        {
            if (player != null)
            {
                cam.Follow = player.transform;
                cam.LookAt = player.transform;
            }
            else
            {
                Debug.LogWarning("⚠️ Player belum ada di scene saat kamera setup.");
            }
        }
    }
}

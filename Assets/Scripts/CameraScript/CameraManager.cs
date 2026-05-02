using UnityEngine;
using Unity.Cinemachine;
//using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [SerializeField] private CameraData[] cineCam;
    private CinemachineCamera currentCamera;


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

        foreach (var data in cineCam)
        {   
            //if data cam = null then continue
            if (data.cam == null)
            {
                Debug.Log("Camera belum di Assign!");
                continue;
            }

            //if data followPlayer true then set follow to player
            if (data.followPlayer)
            {
                if (player != null)
                {
                    data.cam.Follow = player.transform;
                    data.cam.LookAt = player.transform;
                }
                else
                {
                    Debug.LogWarning("⚠️ Player belum ada di scene saat kamera setup.");
                }
            }
        }
    }

    public void SwapCamera(CinemachineCamera camLeft, CinemachineCamera camRight, Vector2 exitDir)
    {
        //set current camera to camLeft
        if (currentCamera == null)
        {
            currentCamera = camLeft;
        }
        
        //if current camera from left and trigger exit was on the right
        if (currentCamera == camLeft && exitDir.x > 0f)
        {
            //active new camera
            camRight.Priority = 20;

            //deactive old camera
            camLeft.Priority = 10;

            //set current camera
            currentCamera = camRight;
        }

        //if current camera from right and trigger exit was on the left
        else if (currentCamera == camRight && exitDir.x < 0f)
        {
            //active new camera
            camLeft.Priority = 20;

            //deactive old camera
            camRight.Priority = 10;
            
            //set current camera
            currentCamera = camLeft;
        }
    }
}

using Unity.Cinemachine;
using UnityEngine;

public class CameraControlTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineCamera CameraLeft;
    [SerializeField] private CinemachineCamera CameraRight;

    private Collider2D _coll;
    
    private void Awake()
    {
        _coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //position of player - middle point of the collider then normalize
            Vector2 exitDirection = (collision.transform.position - _coll.bounds.center).normalized;

            CameraManager.Instance.SwapCamera(CameraLeft, CameraRight, exitDirection);
        }
    }
}

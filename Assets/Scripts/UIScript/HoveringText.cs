using UnityEngine;

public class HoveringText : MonoBehaviour
{
    public float speed = 2f;
    public float height = 10f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.unscaledTime * speed) * height;
        transform.localPosition = startPos + Vector3.up * y;
    }
}

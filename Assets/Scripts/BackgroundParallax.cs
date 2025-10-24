using UnityEngine;

public class BackgroundParallax : MonoBehaviour
{
    private float lenght, startPos;
    public GameObject cam;
    public float ParallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        //lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        // float temp = (cam.transform.position.x * (1 - ParallaxEffect));
        float dist = (cam.transform.position.x * ParallaxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

    }
}

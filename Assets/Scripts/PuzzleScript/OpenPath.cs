using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class OpenPath : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private float fadeSpeed = 4f;

    public void Open()
    {
        StartCoroutine(FadeAndRemove());
    }

    private IEnumerator FadeAndRemove()
    {
        Color c = tilemap.color;

        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * fadeSpeed;
            tilemap.color = c;
            yield return null;
        }

        tilemap.ClearAllTiles(); // 🔥 langsung clear semua

        c.a = 1f;
        tilemap.color = c;
    }
}

using UnityEngine;
using System.Collections;

public class SceneExit : MonoBehaviour
{
    [SerializeField] string nextSceneName;
    [SerializeField] string entryPointID; // ID pintu masuk di scene berikutnya
    [SerializeField] float delay = 0.2f;

    private bool isTransitioning = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;
        if (other.CompareTag("Player"))
        {
            isTransitioning = true;
            StartCoroutine(TransitionToNextScene());
        }
    }

    IEnumerator TransitionToNextScene()
    {
        // Optional: play fade-out animation here
        yield return new WaitForSeconds(delay);

        // Panggil SceneTransitionManager yang modular
        SceneTransitionManager.LoadScene(nextSceneName, entryPointID);
    }
}

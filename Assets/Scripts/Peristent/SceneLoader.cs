// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;

// public class SceneLoader : MonoBehaviour
// {
//     public static SceneLoader Instance;

//     void Awake()
//     {
//         Debug.Log("Script aktif di: " + gameObject.name);
//         if (Instance != null)
//         {
//             Destroy(gameObject); return;
//         }

//         Instance = this;
//     }

//     public void LoadScene(string sceneName)
//     {
//         StartCoroutine(LoadSceneRoutine(sceneName));
//     }

//     private IEnumerator LoadSceneRoutine(string newScene)
//     {
//         Scene current = SceneManager.GetActiveScene();

//         yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
//         SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));

//         if (current.name != "C_PersistentScene")
//             yield return SceneManager.UnloadSceneAsync(current);
//     }
// }
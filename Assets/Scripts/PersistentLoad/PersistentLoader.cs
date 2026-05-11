using UnityEngine;
using UnityEngine.SceneManagement;

public static class PersistentLoader 
{  
    private const string PersistentScene = "C_PersistentScene";
    private const string MainScene = "00_MainMenuScene";
    private const string ToBeContinueScene = "Z_ToBeContinue";   
                                                        
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    
    private static void Init() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureLoadPersistent(SceneManager.GetActiveScene());
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureLoadPersistent(scene);
    }


    private static void EnsureLoadPersistent(Scene scene) 
    {
        if (scene.name == MainScene || scene.name == PersistentScene || scene.name == ToBeContinueScene )
            return;
        
        if (!SceneManager.GetSceneByName(PersistentScene).isLoaded)
        {
            SceneManager.LoadSceneAsync(PersistentScene, LoadSceneMode.Additive);
        }
    }
}

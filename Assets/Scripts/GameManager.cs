using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void GameOver()
    {
        GameOverUI.Instance.Show();
    }

    public void RespawnPlayer()
    {
        Time.timeScale = 1f;    
        string currentLevel = SceneManager.GetActiveScene().name;
        // Panggil SceneTransitionManager untuk reload level
        SceneTransitionManager.LoadScene(currentLevel, "Start"); 
        Debug.Log(currentLevel);
        Debug.Log("Respawn player");
    }
}

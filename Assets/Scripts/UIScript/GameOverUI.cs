using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    [Header("UI")]
    public GameObject redOverlay;
    public GameObject gameOverText;
     public GameObject buttonGroup;

    void Awake()
    {
        Instance = this;
        ResetUI();
    }

    void ResetUI()
    {
        redOverlay.SetActive(false);
        gameOverText.SetActive(false);
        buttonGroup.SetActive(false);
    }

    public void Show()
    {
        Time.timeScale = 1f;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        redOverlay.SetActive(true);
        gameOverText.SetActive(true);
        buttonGroup.SetActive(false);

        yield return new WaitForSecondsRealtime(4f);

        gameOverText.SetActive(false);
        buttonGroup.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        ResetUI();
        GameManager.Instance.RespawnPlayer();
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        ResetUI();
        SceneManager.LoadScene("00_MainMenuScene");
    }
}

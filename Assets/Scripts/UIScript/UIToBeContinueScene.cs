using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class UIToBeContinueScene : MonoBehaviour
{
    [Header("UI")]
    public GameObject tobecontinueText;
    public GameObject ThankyouText;
    public GameObject button;

    [Header("Navigation Settings")]
    [SerializeField] private GameObject firstSelectedButton;

    void Start()
    {
        // Pastikan waktu berjalan normal
        Time.timeScale = 1f;

        // Beri fokus awal agar bisa langsung digerakkan pakai WASD/Gamepad
        if (firstSelectedButton != null)
        {
            // Reset dulu untuk jaga-jaga
            EventSystem.current.SetSelectedGameObject(null);
            // Set fokus ke tombol Play (atau tombol pilihanmu)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }

        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        tobecontinueText.SetActive(true);
        ThankyouText.SetActive(true);
        button.SetActive(false);

        yield return new WaitForSecondsRealtime(3f);

        tobecontinueText.SetActive(false);
        ThankyouText.SetActive(false);
        button.SetActive(true);
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        // 1. Putuskan semua langganan event yang bisa bikin bug
        SceneManager.sceneLoaded -= SceneTransitionManager.OnSceneLoaded;

        // Jika BGMManager pakai event sceneLoaded juga, putuskan di sini
        SceneManager.sceneLoaded -= BGMManager.instance.OnSceneLoaded;

        // 2. Bersihkan data transisi agar tidak nyangkut (Stale Data)
        SceneTransitionManager.ResetData();
        SceneManager.LoadScene("00_MainMenuScene");
    }
}

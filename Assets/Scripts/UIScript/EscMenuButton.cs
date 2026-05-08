using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EscMenuButton : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset controls;

    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;

    //Input Actions UI
    private InputAction pauseAction;
    private bool isPaused;

    void Awake()
    {
        // Pastikan TimeScale normal setiap kali script ini aktif
        Time.timeScale = 1f;

        var uiMap = controls.FindActionMap("UI", true);
        pauseAction = uiMap.FindAction("Pause", true);
    }

    void OnEnable()
    {
        pauseAction.Enable();
        // Berlangganan event performed
        pauseAction.performed += OnPausePerformed;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        pauseAction.Disable();
        // WAJIB: Unsubscribe di OnDisable agar tidak double-register saat scene reload
        pauseAction.performed -= OnPausePerformed;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        // Cari panel jika tiba-tiba null (misal setelah pindah scene)
        if (pauseMenuUI == null)
        {
            FindPausePanel();
        }

        if (pauseMenuUI != null)
        {
            TogglePause();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    // RESET STATUS
    Time.timeScale = 1f;
    isPaused = false;

    // WAJIB: Cari ulang panel di scene yang baru di-load
    // Karena panel yang lama sudah hilang/destroy
    GameObject panel = GameObject.FindWithTag("PauseMenu");
    if (panel != null)
    {
        pauseMenuUI = panel;
        pauseMenuUI.SetActive(false);
    }
}

    void FindPausePanel()
    {
        // Mencari objek dengan tag "PauseMenu"
        GameObject panel = GameObject.FindWithTag("PauseMenu");
        if (panel != null)
        {
            pauseMenuUI = panel;
            pauseMenuUI.SetActive(false);
            Debug.Log("Pause Panel ditemukan dan di-link otomatis.");
        }
        else
        {
            Debug.LogWarning("Peringatan: Tidak ada objek dengan tag 'PauseMenu' di scene ini!");
        }
    }

    void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (pauseMenuUI == null) return;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuUI == null) return;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // =====================
    // BUTTON UI
    // =====================

    public void RetryButton()
    {
        // Pastikan waktu jalan dulu sebelum pindah scene
        Time.timeScale = 1f;
        isPaused = false;
        
        GameManager.Instance.RespawnPlayer();
    }

    public void ExitToMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("00_MainMenuScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
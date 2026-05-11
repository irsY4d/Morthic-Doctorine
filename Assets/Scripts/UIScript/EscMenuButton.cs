using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EscMenuButton : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionAsset controls;

    [Header("UI")]
    [SerializeField] private GameObject pauseMenuUI;

    [Tooltip("Tarik tombol 'Retry' ke sini agar otomatis tersorot saat pause")]
    [SerializeField] private GameObject firstSelectedButton;

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

            // Jika belum di-set di inspector, cari tombol pertama (Retry) secara otomatis
            if (firstSelectedButton == null)
            {
                firstSelectedButton = pauseMenuUI.GetComponentInChildren<Button>()?.gameObject;
            }
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

            // Ambil tombol pertama yang ketemu (biasanya Retry)
            firstSelectedButton = pauseMenuUI.GetComponentInChildren<Button>()?.gameObject;
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

        // --- NAVIGASI FIX ---
        EventSystem.current.SetSelectedGameObject(null);

        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
        else
        {
            // Fail-safe: cari lagi jika tiba-tiba hilang
            GameObject btn = pauseMenuUI.GetComponentInChildren<Button>()?.gameObject;
            EventSystem.current.SetSelectedGameObject(btn);
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI == null) return;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Bersihkan seleksi agar input keyboard tidak 'nyangkut' di UI saat main
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);    
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
        // 1. Putuskan semua langganan event yang bisa bikin bug
        SceneManager.sceneLoaded -= SceneTransitionManager.OnSceneLoaded;

        // Jika BGMManager pakai event sceneLoaded juga, putuskan di sini
        SceneManager.sceneLoaded -= BGMManager.instance.OnSceneLoaded;

        // 2. Bersihkan data transisi agar tidak nyangkut (Stale Data)
        SceneTransitionManager.ResetData();
        SceneManager.LoadScene("00_MainMenuScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
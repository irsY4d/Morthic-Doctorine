using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
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
    }
    
    public void OnClickPlay()
    {
        SceneManager.LoadScene("0_PrologScene");
    }

    public void OnClickCredit()
    {
        print("Creeedit");
    }

    public void OnClickExit()
    {

        Application.Quit();
    }
}

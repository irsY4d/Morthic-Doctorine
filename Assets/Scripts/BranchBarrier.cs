using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class BranchBarrier : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Tarik semua musuh yang harus mati ke sini")]
    [SerializeField] private List<EnemyController> enemiesToWatch;

    [Header("UI Quest")]
    [SerializeField] private TextMeshProUGUI progressText;

    private int totalEnemy;
    private int deadEnemyCount = 0;
    private bool isOpened = false;

    void Start()
    {
        // Hitung total musuh yang didaftarkan di awal
        totalEnemy = enemiesToWatch.Count;
        
        if (totalEnemy == 0)
        {
            Debug.LogWarning("Opps! Kamu belum memasukkan enemy ke list di BranchBarrier.");
        }

        if (progressText != null)
        {
            progressText.gameObject.SetActive(true);
            UpdateProgressText();
        }
        else
        {
            Debug.LogWarning("Progress text UI belum di-assign di BranchBarrier.");
        }
    }

    void Update()
    {
        if (isOpened || enemiesToWatch.Count == 0) return;

        // Cek list musuh dari belakang (agar aman saat menghapus list)
        for (int i = enemiesToWatch.Count - 1; i >= 0; i--)
        {
            // Jika musuh sudah mati (isDead dari script EnemyController)
            if (enemiesToWatch[i] == null || enemiesToWatch[i].IsDead)
            {
                deadEnemyCount++;
                enemiesToWatch.RemoveAt(i); // Hapus dari pantauan agar tidak dihitung lagi
                Debug.Log($"Enemy mati! Progress: {deadEnemyCount}/{totalEnemy}");
            }
        }

        UpdateProgressText();

        // Cek apakah semua target sudah mati
        if (deadEnemyCount >= totalEnemy && totalEnemy > 0)
        {
            OpenPath();
            progressText.gameObject.SetActive(false);
        }
    }

    private void UpdateProgressText()
    {
        if (progressText == null) return;

        progressText.text = $"Eliminate Foes : {deadEnemyCount}/{totalEnemy}";
    }

    private void OpenPath()
    {
        isOpened = true;
        Debug.Log("Semua musuh target mati. Branch hancur!");
        
        // Kamu bisa tambah efek hancur/animasi di sini
        Destroy(gameObject);
    }
}
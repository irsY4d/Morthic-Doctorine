using UnityEngine;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// BranchBarrier mengatur objective membereskan musuh target untuk membuka path.
/// Script ini memonitor musuh, update UI progress, dan mendaftarkan quest di GameManager.
/// </summary>
public class BranchBarrier : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Tarik semua musuh yang harus mati ke sini")]
    [SerializeField] private List<EnemyController> enemiesToWatch;

    [Header("Quest Settings")]
    [Tooltip("Gunakan unique quest ID agar GameManager dapat membedakan quest berbeda.")]
    [SerializeField] private string questId = "branch_barrier_quest";
    [SerializeField] private string questTitle = "Eliminate the Branch Barrier Foes";

    [Header("UI Quest")]
    [SerializeField] private TextMeshProUGUI progressText;

    private int totalEnemy;
    private int deadEnemyCount = 0;
    private bool isOpened = false;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            QuestData existingQuest = GameManager.Instance.GetQuest(questId);

            if (existingQuest != null && existingQuest.IsCompleted)
            {
                // Hancurkan semua musuh yang ada di list pemantauan
                foreach (var enemy in enemiesToWatch)
                {
                    if (enemy != null) Destroy(enemy.gameObject);
                }

                // Lalu hancurkan diri sendiri (Barrier)
                Destroy(gameObject);
                return;
            }
        }
        // Hitung total musuh yang didaftarkan di awal.
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

        // Daftarkan quest ke GameManager agar quest dapat dipantau secara global.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterQuest(questId, questTitle, totalEnemy);
        }
    }

    void Update()
    {
        if (isOpened || enemiesToWatch.Count == 0) return;

        bool progressChanged = false;

        // Cek list musuh dari belakang agar aman saat menghapus elemen.
        for (int i = enemiesToWatch.Count - 1; i >= 0; i--)
        {
            if (enemiesToWatch[i] == null || enemiesToWatch[i].IsDead)
            {
                deadEnemyCount++;
                enemiesToWatch.RemoveAt(i); // Hapus dari pantauan agar tidak dihitung lagi.
                progressChanged = true;
                Debug.Log($"Enemy mati! Progress: {deadEnemyCount}/{totalEnemy}");
            }
        }

        if (progressChanged && GameManager.Instance != null)
        {
            GameManager.Instance.SetQuestProgress(questId, deadEnemyCount);
        }

        UpdateProgressText();

        if (deadEnemyCount >= totalEnemy && totalEnemy > 0)
        {
            OpenPath();
            if (progressText != null)
            {
                progressText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Mengupdate teks progress UI.
    /// </summary>
    private void UpdateProgressText()
    {
        if (progressText == null) return;

        progressText.text = $"Eliminate Foes : {deadEnemyCount}/{totalEnemy}";
    }

    /// <summary>
    /// Membuka path dan menyelesaikan quest ketika semua musuh target telah mati.
    /// </summary>
    private void OpenPath()
    {
        isOpened = true;
        Debug.Log("Semua musuh target mati. Branch hancur!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CompleteQuest(questId);
        }

        // Tambahkan efek visual/animasi di sini jika ingin.
        Destroy(gameObject);
    }
}

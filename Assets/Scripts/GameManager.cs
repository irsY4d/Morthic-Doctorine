using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// <summary>
/// Simple data container for one quest's state.
/// </summary>
[Serializable]
public class QuestData
{
    public string questId;
    public string title;
    public string description;
    public int targetAmount;
    public int currentAmount;

    public bool IsCompleted => targetAmount > 0 && currentAmount >= targetAmount;
    public float Progress => targetAmount > 0 ? (float)currentAmount / targetAmount : 0f;
}

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Static reference agar script lain mudah mengakses GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    [Header("Quest System")]
    [Tooltip("Daftar quest aktif yang dapat di-update oleh object lain.")]
    [SerializeField] private List<QuestData> quests = new List<QuestData>();

    /// <summary>
    /// Dipanggil setiap kali progress quest berubah.
    /// </summary>
    public event Action<QuestData> OnQuestUpdated;

    /// <summary>
    /// Dipanggil saat quest selesai.
    /// </summary>
    public event Action<QuestData> OnQuestCompleted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Cari quest berdasarkan questId.
    /// </summary>
    public QuestData GetQuest(string questId)
    {
        return quests.Find(q => q.questId == questId);
    }

    /// <summary>
    /// Daftarkan quest baru atau update quest yang sudah ada.
    /// </summary>
    public void RegisterQuest(string questId, string title, int targetAmount, string description = "")
    {
        if (string.IsNullOrEmpty(questId))
        {
            Debug.LogWarning("Quest ID tidak boleh kosong saat mendaftarkan quest.");
            return;
        }

        QuestData quest = GetQuest(questId);
        if (quest == null)
        {
            quest = new QuestData
            {
                questId = questId,
                title = title,
                description = description,
                targetAmount = targetAmount,
                currentAmount = 0
            };
            quests.Add(quest);
        }
        else
        {
            quest.title = title;
            quest.description = description;
            quest.targetAmount = targetAmount;
            quest.currentAmount = Mathf.Min(quest.currentAmount, targetAmount);
        }

        OnQuestUpdated?.Invoke(quest);
    }

    /// <summary>
    /// Set quest progress secara langsung ke angka tertentu.
    /// </summary>
    public void SetQuestProgress(string questId, int currentAmount)
    {
        QuestData quest = GetQuest(questId);
        if (quest == null)
        {
            Debug.LogWarning($"Quest {questId} belum terdaftar di GameManager.");
            return;
        }

        quest.currentAmount = Mathf.Clamp(currentAmount, 0, quest.targetAmount);
        OnQuestUpdated?.Invoke(quest);

        if (quest.IsCompleted)
        {
            CompleteQuest(questId);
        }
    }

    /// <summary>
    /// Menambah progress quest, misalnya ketika musuh mati atau item dikumpulkan.
    /// </summary>
    public void AddQuestProgress(string questId, int amount = 1)
    {
        QuestData quest = GetQuest(questId);
        if (quest == null)
        {
            Debug.LogWarning($"Quest {questId} belum terdaftar di GameManager.");
            return;
        }

        if (quest.IsCompleted) return;

        quest.currentAmount = Mathf.Clamp(quest.currentAmount + amount, 0, quest.targetAmount);
        OnQuestUpdated?.Invoke(quest);

        if (quest.IsCompleted)
        {
            CompleteQuest(questId);
        }
    }

    /// <summary>
    /// Menandai quest sebagai selesai dan memanggil event completion.
    /// </summary>
    public void CompleteQuest(string questId)
    {
        QuestData quest = GetQuest(questId);
        if (quest == null) return;

        if (!quest.IsCompleted)
        {
            quest.currentAmount = quest.targetAmount;
        }

        OnQuestCompleted?.Invoke(quest);
        Debug.Log($"Quest selesai: {quest.title} ({quest.questId})");
    }

    /// <summary>
    /// Menampilkan UI Game Over.
    /// </summary>
    public void GameOver()
    {
        GameOverUI.Instance.Show();
    }

    /// <summary>
    /// Reload level yang sedang aktif saat player respawn.
    /// </summary>
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

using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// Simple data container for one quest's state.
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
    /// Static reference agar script lain mudah mengakses GameManager.
    public static GameManager Instance { get; private set; }

    [Header("Quest System")]
    [Tooltip("Daftar quest aktif yang dapat di-update oleh object lain.")]
    [SerializeField] private List<QuestData> quests = new List<QuestData>();

    public event Action<QuestData> OnQuestUpdated; // Dipanggil setiap kali progress quest berubah.
    public event Action<QuestData> OnQuestCompleted; // Dipanggil saat quest selesai.

    [Header("Cutscene Tracker")]
    public List<string> completedCutscenes = new List<string>();

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

    #region Quest System
    /// Cari quest berdasarkan questId.
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
    #endregion

    #region Cutscene List
    public bool IsCutsceneDone(string id)
    {
        return completedCutscenes.Contains(id);
    }

    public void MarkCutsceneAsDone(string id)
    {
        if (!completedCutscenes.Contains(id))
        {
            completedCutscenes.Add(id);
            Debug.Log("Cutscene disimpan ke memori: " + id);
        }
    }
    #endregion 

    #region Game Over System
    /// Menampilkan UI Game Over.
    public void GameOver()
    {
        GameOverUI.Instance.Show();
    }
    #endregion

    #region Respawn System
    /// Reload level yang sedang aktif saat player respawn.
    public string lastCheckpointID; // Simpan ID checkpoint terakhir yang diinjak player
    public string lastCheckpointScene;
    public void RespawnPlayer()
    {
        //Pastikan waktu berjalan normal (jika sebelumnya game di-pause)
        Time.timeScale = 1f;

        //Reset health player sebelum respawn agar tidak mati lagi saat muncul
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ResetHealth();
            }
            PlayerCombat combat = player.GetComponent<PlayerCombat>();
            if (combat != null)
            {
                // Kita akan buat fungsi ini di PlayerCombat
                combat.ResetCombatStatus();
            }
        }

        // Tentukan scene tujuan respawn
        string targetScene = lastCheckpointScene;

        if (string.IsNullOrEmpty(targetScene))
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.name != "C_PersistentScene")
                {
                    targetScene = s.name;
                    break;
                }
            }
        }

        // Muat ulang scene menggunakan TransitionManager agar persistent tetap aman
        // Kita kirim flag isRespawn = true agar sistem mencari 'RespawnChekpoint' bukan 'PlayerSpawnPoint'
        if (!string.IsNullOrEmpty(targetScene))
        {
            Debug.Log("Respawning Player ke Scene: " + targetScene + " ID: " + lastCheckpointID);
            SceneTransitionManager.LoadScene(targetScene, lastCheckpointID, true);
        }

        ResetAllLayers(player);
    }

    void ResetAllLayers(GameObject obj)
    {
        // Cek apakah ini objek hitbox pedang (bisa dicek lewat nama atau komponen)
        if (obj.name.Contains("Sword"))
            obj.layer = LayerMask.NameToLayer("PlayerAttack");
        else if (obj.name.Contains("Feet"))
            obj.layer = LayerMask.NameToLayer("Feet");
        else
            obj.layer = LayerMask.NameToLayer("Player");

        // Ulangi untuk semua anak-anaknya secara otomatis
        foreach (Transform child in obj.transform)
        {
            ResetAllLayers(child.gameObject);
        }
    }
    #endregion
}

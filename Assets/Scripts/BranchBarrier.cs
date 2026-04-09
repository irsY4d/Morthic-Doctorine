using UnityEngine;
using System.Collections.Generic;

public class BranchBarrier : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Tarik semua musuh yang harus mati ke sini")]
    [SerializeField] private List<EnemyController> enemiesToWatch;

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

        // Cek apakah semua target sudah mati
        if (deadEnemyCount >= totalEnemy && totalEnemy > 0)
        {
            OpenPath();
        }
    }

    private void OpenPath()
    {
        isOpened = true;
        Debug.Log("Semua musuh target mati. Branch hancur!");
        
        // Kamu bisa tambah efek hancur/animasi di sini
        Destroy(gameObject);
    }
}
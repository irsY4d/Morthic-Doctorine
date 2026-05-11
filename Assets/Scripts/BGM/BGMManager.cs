using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public AudioClip music;
}

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;
    private AudioSource audioSource;
    [SerializeField] private float fadeDuration = 1.5f;
    private Coroutine fadeCoroutine;


    public SceneMusic[] sceneMusics;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void Start()
    {
        // Cek BGM untuk scene yang aktif saat ini saat pertama kali game jalan
        CheckAndPlayBGM(SceneManager.GetActiveScene().name);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlayBGM(scene.name);
    }
    void CheckAndPlayBGM(string sceneName)
    {
        foreach (var sm in sceneMusics)
        {
            if (sm.sceneName == sceneName)
            {
                PlayMusic(sm.music);
                return;
            }
        }
    }

    void PlayMusic(AudioClip clip)
    {
        // Cek apakah AudioSource masih ada di memori
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource sudah hancur, membatalkan PlayMusic.");
            return;
        }

        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        // Stop coroutine yang sedang berjalan jika ada
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeTransition(clip));
    }

    private IEnumerator FadeTransition(AudioClip newClip)
    {
        // Tentukan target volume (biasanya 1 atau volume awal kamu)
        float targetVolume = 1f;
        float startVolume = audioSource.volume;

        // 1. FADE OUT (Mengecilkan musik yang sedang putar)
        if (audioSource.isPlaying)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                if (audioSource == null) yield break;
                audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
        }

        // 2. GANTI KLIP & PLAY (Saat volume sudah 0)
        audioSource.clip = newClip;
        audioSource.volume = 0; // Pastikan mulai dari sunyi

        if (newClip != null)
        {
            audioSource.Play();

            // 3. FADE IN (Membesarkan musik baru)
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                if (audioSource == null) yield break;
                audioSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = targetVolume;
        }
    }
}
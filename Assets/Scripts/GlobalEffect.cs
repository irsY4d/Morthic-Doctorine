using UnityEngine;
using System.Collections;

public class GlobalEffect : MonoBehaviour
{
    public static GlobalEffect Instance { get; private set; }

    [Header("Global VFX & SFX")]
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject RegenEffectPrefab;
    [SerializeField] private AudioClip hitImpactSFX;
    [SerializeField] private AudioClip regenSFX;
    [SerializeField] private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnBloodEffect(Vector3 position)
    {
        if (bloodEffectPrefab == null)
        {
            return;
        }

        GameObject blood = Instantiate(bloodEffectPrefab, position, Quaternion.identity);
        Destroy(blood, 0.1f); // hancurkan prefab yang di-spawn, bukan GlobalEffect!
    }

    public void RegenEffect(Vector3 position, Transform followTarget = null)
    {
        GameObject regen = Instantiate(RegenEffectPrefab, position, Quaternion.identity);
        Destroy(regen, 1f);

        if (followTarget != null)
        {
            regen.transform.SetParent(followTarget);
        }
    }

    public void PlayHitSFX()
    {
        if (hitImpactSFX == null || audioSource == null) return;
        audioSource.pitch = Random.Range(0.95f, 1.05f); // variasi biar natural
        audioSource.PlayOneShot(hitImpactSFX);
    }

    public void PlayHealSFX()
    {
        if (regenSFX == null || audioSource == null) return;
        audioSource.pitch = Random.Range(0.95f, 1.05f); // variasi biar natural
        audioSource.PlayOneShot(regenSFX);
    }

}

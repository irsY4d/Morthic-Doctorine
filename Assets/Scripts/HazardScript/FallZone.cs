using UnityEngine;

public class FallZone : MonoBehaviour
{
    [SerializeField] Transform respawnPoint;
    [SerializeField] float respawnDamage = 2f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // pindahkan player ke respawn point
            other.transform.position = respawnPoint.position;

            // reset velocity biar nggak kebawa momentum jatuh
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // panggil fungsi TakeDamage dari PlayerHealth
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(respawnDamage);
            }
        }
    }
}

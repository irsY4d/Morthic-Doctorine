using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] AttackData damage;

    private Rigidbody2D rb;
    private bool hasHitPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // cari player di scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // arah dari fireball ke player
            Vector2 direction = (player.transform.position - transform.position).normalized;

            // kasih velocity sesuai arah
            rb.linearVelocity = direction * speed;

            // rotate fireball biar menghadap ke arah terbang
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // auto destroy
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("Player kena fireball!");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage.damageAmount);
                Debug.Log($" Fireball damage : {damage.damageAmount}");
                hasHitPlayer = true;
            }
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (other.tag == "SkyGround")
            {
                return;
            }
            
            Debug.Log("Tanah");
            Destroy(gameObject);
        }
    }
}

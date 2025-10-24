using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EnemyController enemyController; // biar bisa stop patrol saat chasing
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] AudioClip clawAttack;

    [Header("Chase Settings")]
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float attackRange = 1.2f;

    [Header("Attack Settings")]
    [SerializeField] BoxCollider2D clawAttackHitbox;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] AttackData clawattackData; // ScriptableObject buat damage

    private float attackTimer = 0f;
    private Transform player;
    private bool hasHitPlayer = false;
    private AudioSource audioSource;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (enemyController == null)
            enemyController = GetComponent<EnemyController>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (enemyController.IsDead) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRadius)
        {
            ChasePlayer(distance);
        }
        else
        {
            enemyController.ResumePatrol(); // ⬅️ Kembali patrol kalau player kabur
        }
    }


    void ChasePlayer(float distance)
    {
        // Stop patrol ketika chase
        enemyController.StopPatrol();

        if (distance > attackRange)
        {
            // Jalan ke arah player
            animator.SetBool("isWalking", true);

            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(dir.x * enemyController.EnemySpeed, rb.linearVelocity.y);

            // flip sprite sesuai arah
            if (dir.x != 0)
            {
                Vector3 scale = transform.localScale;
                scale.x = dir.x > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                transform.localScale = scale;
            }
        }
        else
        {
            // Dalam jarak attack
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("isAttacking");
        Invoke(nameof(DisableHitbox), 0.2f);
        Debug.Log($"Enemy attacks with {clawattackData.attackName} → {clawattackData.damageAmount} dmg");
    }


    public void EnableHitbox()
    {
        clawAttackHitbox.enabled = true;
        hasHitPlayer = false;
    }

    public void DisableHitbox()
    {
        clawAttackHitbox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!clawAttackHitbox.enabled || hasHitPlayer) return;
        //if (gameObject.layer != LayerMask.NameToLayer("Player")) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(clawattackData.damageAmount);
                hasHitPlayer = true;
                Debug.Log($"Enemy hit player once with {clawattackData.attackName}");
            }
        }
    }

    public void clawSFX()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clawAttack);
        }
    }
}

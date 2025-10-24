using UnityEngine;
using System.Collections;

public class RevenantAttack : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float attackRange = 1.2f;

    [Header("Attack Settings")]
    [SerializeField] BoxCollider2D attackHitbox;
    [SerializeField] float attackCooldown = 1f;
    [SerializeField] AttackData revenantAttackData;

    [Header("References")]
    [SerializeField] Animator animator;
    [SerializeField] EnemyController enemyController;
    [SerializeField] AudioClip attackSFX;

    public Transform startPosition;

    private Rigidbody2D rb;
    private Transform player;
    private AudioSource audioSource;
    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool hasHitPlayer = false;
    private bool chase = false;
    

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
        if (player == null || enemyController.IsDead)
        {
            Debug.Log("sudah mati");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Player masuk area deteksi → mulai ngejar
        if (distanceToPlayer <= detectionRadius)
        {
            chase = true;
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            if (chase)
            {
                chase = false;
                animator.SetBool("isWalking", true);
            }

            ReturnToStart();
        }

    }

    void ChasePlayer(float distance)
    {
        if (isAttacking) return;

        if (distance > attackRange)
        {
            Vector2 target = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, enemyController.EnemySpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);
            Flip(target.x);
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

    void ReturnToStart()
    {
        Vector2 target = new Vector2(startPosition.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, enemyController.EnemySpeed * Time.deltaTime);
        Flip(target.x);
        if (Vector2.Distance(transform.position, target) < 0.05f)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }

    private void Flip(float targetX)
    {
        if (player == null) return;

        if (targetX > transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("isAttacking");
        Debug.Log($"Enemy attacks with {revenantAttackData.attackName} → {revenantAttackData.damageAmount} dmg");
    }

    public void EnableHitbox()
    {
        attackHitbox.enabled = true;
        hasHitPlayer = false;
    }
    public void DisableHitbox()
    {
        attackHitbox.enabled = false;
    }

    public void AttackSFX()
    {
        if (attackSFX != null)
            audioSource.PlayOneShot(attackSFX);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.enabled || hasHitPlayer) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(revenantAttackData.damageAmount);
                hasHitPlayer = true;
                Debug.Log($"Enemy hit player once with {revenantAttackData.attackName}");
            }
        }
    }

    // event di akhir animasi serang
    public void EndAttack()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

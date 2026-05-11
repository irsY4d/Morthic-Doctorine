using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Data")]
    [SerializeField] EnemyData data;
    [SerializeField] protected Transform leftPoint;
    [SerializeField] protected Transform rightPoint;
    [SerializeField] Animator animator;
    [SerializeField] float idleDuration = 2f;

    protected Rigidbody2D rb;
    protected Vector2 startPos;
    protected bool movingRight = true;

    private EnemyData runtimeData;
    private bool isIdle = false;
    private float idleTimer = 0f;
    private bool isDead = false;
    private bool isHit = false;
    private bool isChasing = false;
    private float currentHealth;

    [Header("Combat")]
    [SerializeField] float hitRecoveryDuration = 0.4f;

    public bool IsDead => isDead;
    public bool IsChasing => isChasing;
    public bool HasSuperArmor => data.isBoss;
    public float EnemySpeed => data.moveSpeed;
    public string EnemyName => data.enemyName;

    void Awake()
    {
        runtimeData = Instantiate(data);
        currentHealth = runtimeData.maxHealth;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        movingRight = transform.localScale.x < 0;
    }

    protected virtual void Update()
    {
        //Debug.Log($"Enemy {EnemyName} → isDead: {isDead}, isHit: {isHit}, isChasing: {isChasing}");
        if (isDead || isChasing) return;

        if (data.canPatrol && !data.isBoss) // hanya enemy yang bisa patrol
            Patrol();

    }

    public virtual void TakeDamage(float dmg)
    {
        if (isDead) return;

        isHit = true;
        currentHealth -= dmg;
        Debug.Log($"Enemy {data.enemyName} took {dmg} damage → {currentHealth} HP left");

        if (!HasSuperArmor) 
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("Hit");
            Vector3 offset = new Vector3(0.2f, 0.1f, 0f); // tweak sesuai kebutuhan
            GlobalEffect.Instance.SpawnBloodEffect(transform.position + offset);
            GlobalEffect.Instance.PlayHitSFX();
            StartCoroutine(ResetHit());
        }
        else
        {
            StartCoroutine(FlashRed());
            StartCoroutine(ResetHit());
        }

        if (currentHealth <= 0)
        {
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("Dead");
        }
    }

    IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white;
        }
    }

    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(hitRecoveryDuration);
        isHit = false;
    }

    public void DestroySelf()
    {
        Debug.Log($"{EnemyName} destroyed!");
        Destroy(gameObject);
    }

    // -------------------
    // 🔹 Fungsi Modular Flip
    // -------------------
    public virtual void Flip(bool faceRight)
    {
        movingRight = faceRight;
        transform.localScale = new Vector3(
            faceRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    // 🔹 Menghadap player (buat Necromancer / Ranged Enemy)
    public virtual void FacePlayer(Transform player)
    {
        if (player == null) return;

        bool playerIsRight = player.position.x > transform.position.x;
        Flip(playerIsRight);
    }

    protected virtual void Patrol()
    {
        if (isHit || isDead) return;

        if (isIdle)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                idleTimer = 0f;
                Flip(!movingRight);
            }
        }
        else
        {
            animator.SetBool("isWalking", true);

            float dir = movingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * data.moveSpeed, rb.linearVelocity.y);

            // cek apakah sampai ke titik batas
            if (movingRight && transform.position.x >= rightPoint.position.x)
            {
                isIdle = true;
                rb.linearVelocity = Vector2.zero;
            }
            else if (!movingRight && transform.position.x <= leftPoint.position.x)
            {
                isIdle = true;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }


    public void StopPatrol()
    {
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        isIdle = false;
        isChasing = true;
    }

    public void ResumePatrol()
    {
        isChasing = false;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isWalking", false);
        Flip(movingRight);
        //Debug.Log("Continue Patroling");
    }
}

using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float HealthPoint = 4f;
    [SerializeField] Animator animator;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float moveDistance = 4f;
    [SerializeField] float idleDuration = 2f;
    [SerializeField] bool enemyBoss = false;

    private Rigidbody2D rb;
    private Vector2 startPos;
    private bool movingRight = true;
    private bool isIdle = false;
    private float idleTimer = 0f;
    private bool isDead = false;
    private bool isHit = false;
    private bool isChasing = false;

    public bool IsDead => isDead;
    public bool IsChasing => isChasing;
    public bool HasSuperArmor => enemyBoss;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        movingRight = transform.localScale.x < 0;
    }

    void Update()
    {
        if (isDead || isChasing) return;

        Patrol();
    }

    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        isHit = true;
        HealthPoint -= dmg;
        Debug.Log($"Enemy took {dmg} damage → {HealthPoint} HP left");

        if (!HasSuperArmor)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("Hit");
        }
        else
        {
            StartCoroutine(FlashRed());
        }

        if (HealthPoint <= 0)
        {
            isDead = true;
            rb.linearVelocity = Vector2.zero;
            animator.SetTrigger("Dead");
            Debug.Log("Enemy Dead");
        }
    }   

    IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = Color.white; // balik ke normal
        }
    }

    // void ResetHit()
    // {
    //     isHit = false;
    // }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }


    void Patrol()
    {
        if (isHit || isDead) // kalau lagi dipukul → stop total
        {
            rb.linearVelocity = Vector2.zero;
            //animator.SetBool("isWalking", false);
            return;
        }

        if (isIdle) // kalau lagi idle → berhenti
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            idleTimer += Time.deltaTime;
            if (idleTimer >= idleDuration)
            {
                isIdle = false;
                idleTimer = 0f;
                movingRight = !movingRight;

                // flip sprite
                transform.localScale = new Vector3(
                    movingRight ? -Mathf.Abs(transform.localScale.x) : Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
            }
        }
        else // kalau jalan
        {
            animator.SetBool("isWalking", true);

            float dir = movingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);

            // cek batas patrol
            if ((movingRight && transform.position.x >= startPos.x + moveDistance / 2f) ||
                (!movingRight && transform.position.x <= startPos.x - moveDistance / 2f))
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
        isIdle = false; // biar ga stuck idle
        isChasing = true;
    }
    public void ResumePatrol()
    {
        isChasing = false;
    }

}
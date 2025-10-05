using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class NecromancerAttack : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] float detectionRadius = 5f;
    [SerializeField] float attackRange = 0;
    [SerializeField] float attackCooldown = 0;
    [SerializeField] GameObject NecrofireballPrefab;
    [SerializeField] Transform NecrofireballSpawnPoint;
    [SerializeField] EnemyController enemyController;
    [SerializeField] float chaseSpeed = 2f;

    [SerializeField] Animator animator;

    private Transform player;
    private float cooldownTImer;
    private bool isAttacking = false;
    private Rigidbody2D rb;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        if (isAttacking == true)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        cooldownTImer -= Time.deltaTime;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            enemyController.StopPatrol();
            enemyController.FacePlayer(player);
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);

            // Attack kalau dalam range
            if (distance <= attackRange && cooldownTImer <= 0f)
            {
                FireballAttack();
                cooldownTImer = attackCooldown;
            }
        }
        else
        {
            enemyController.ResumePatrol();
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        Debug.Log("resettttt");
    }


    void FireballAttack()
    {
        Debug.Log("Necromancer casts Fireball!");
        isAttacking = true;
        animator.SetTrigger("fireballAttack");
        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(1f); // durasi animasi fireball
        EndAttack();
    }

    public void spawnFireball()
    {
        Instantiate(NecrofireballPrefab, NecrofireballSpawnPoint.position, NecrofireballSpawnPoint.rotation);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

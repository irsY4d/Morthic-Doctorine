using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float HealthPoint;
    [SerializeField] PlayerAnimationController animationController;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        animationController.GetHit();
        HealthPoint -= damage;
        Debug.Log($"Player took {damage} damage → {HealthPoint} HP left");

        StartCoroutine(DisableMovement());

        if (HealthPoint <= 0)
        {
            rb.linearVelocity = Vector2.zero;
            animationController.SetDeath();
            GetComponent<PlayerMovement>().enabled = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator DisableMovement()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        movement.enabled = false;

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        if (HealthPoint > 0)
        {
            movement.enabled = true;
        }
    }
}

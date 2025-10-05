using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public float HealthPoint;
    [SerializeField] public float maxHealthPoint;
    [SerializeField] PlayerAnimationController animationController;

    public Vector2 knockbackForce = new Vector2(5f, 3f);
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        HealthPoint -= damage;

        animationController.GetHit();
        animationController.SetDeath(false);

        Debug.Log($"Player took {damage} damage → {HealthPoint} HP left");

        ApplyKnockback();

        StartCoroutine(DisableMovement());
        DeadState();
    }

    void DeadState()
    {
        if (HealthPoint <= 0)
        {
            gameObject.layer = LayerMask.NameToLayer("Feet");
            
            rb.linearVelocity = Vector2.zero;
            GetComponent<PlayerMovement>().enabled = false;
            StopAllCoroutines();
            StartCoroutine(DelayedDeathFlag());

            SetLayerRecursively(gameObject, "Feet");
        }
    }

    void ApplyKnockback()
    {
        float direction = transform.localScale.x > 0 ? -1f : 1f; // arah berlawanan dari hadapan
        Vector2 force = new Vector2(knockbackForce.x * direction, knockbackForce.y);
        rb.linearVelocity = Vector2.zero; // reset dulu
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    void SetLayerRecursively(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layerName);
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

    IEnumerator DelayedDeathFlag()
    {
        yield return new WaitForSeconds(0.2f); // kasih waktu animator masuk ke GotHit
        animationController.SetDeath(true);
    }
}

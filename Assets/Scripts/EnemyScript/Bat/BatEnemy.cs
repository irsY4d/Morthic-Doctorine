using System.Data.Common;
using UnityEngine;

public class BatEnemy : EnemyController
{
    
    [Header("Bat Movement")]
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatSpeed = 4f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        
    }

    void FixedUpdate()
    {
         if (IsDead || IsChasing)
        return;
        
        Patrol();
    }

    public override void TakeDamage(float dmg)
    {
        base.TakeDamage(dmg);


        if (IsDead)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Enable physics
            rb.gravityScale = 2f; // Stop movement immediately
        }
    }

    protected override void Patrol()
    {
        // horizontal movement
        float dir = movingRight ? -1f : 1f;

        // vertical floating movement
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        Vector2 targetPos = new Vector2(
            transform.position.x + dir * EnemySpeed * Time.deltaTime,
            startPos.y + yOffset
        );

        rb.MovePosition(targetPos);

        // batas patrol
        if (transform.position.x >= rightPoint.position.x)
        {
            movingRight = true;
            Flip(false);
        }
        else if (transform.position.x <= leftPoint.position.x)
        {
            movingRight = false;
            Flip(true);
        }
    }

    public override void Flip(bool faceRight)
    {
        transform.localScale = new Vector3(
            faceRight ? 1 : -1,
            1,
            1
        );
    }
}

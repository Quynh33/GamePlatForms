using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float detectionRange = 10f; // Range within which the zombie chases the player
    [SerializeField] private LayerMask whatIsGround;
    private float timer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); // Ensure the base class Start() is called if needed
        rb.gravityScale = 12f;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (!isRecoiling)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);

            // Check if the player is within detection range
            if (distanceToPlayer <= detectionRange)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y),
                    speed * Time.deltaTime
                );
            }
        }
    }

    // Update enemy states
    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (getCurrentEnemyState)
        {
            case EnemyStates.Enemy_Idle:
                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Enemy_Flip);
                }

                // Update velocity based on direction
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                break;

            case EnemyStates.Enemy_Flip:
                timer += Time.deltaTime;
                if (timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Enemy_Idle);
                }
                break;
        }
    }
}
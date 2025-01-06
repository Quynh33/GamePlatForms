using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insect : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    [SerializeField] private int hitsBeforeStun ;
    private int currentHitCount = 0;
    float timer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Insect_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if(!PlayerMovement.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Insect_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {   float _dist = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);
        switch (getCurrentEnemyState)
        {
            case EnemyStates.Insect_Idle:
                if(_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Insect_Chase);
                }
                break;
            case EnemyStates.Insect_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerMovement.Instance.transform.position, Time.deltaTime * speed));
                FlipInsect();
                break;
            case EnemyStates.Insect_Stunned:
                rb.velocity = Vector2.zero;  // Dừng mọi chuyển động
                rb.angularVelocity = 0f;     // Dừng xoay nếu có
                timer += Time.deltaTime;
                if (timer > stunDuration)   
                {
                    ChangeState(EnemyStates.Insect_Idle);
                    timer = 0;
                }
                break;
            case EnemyStates.Insect_Death:
                Death(2f);
                break;
        }
    }
    protected override void ChangeCurrentAnimaton()
    {
        anmin.SetBool("Idle", getCurrentEnemyState == EnemyStates.Insect_Idle);
        anmin.SetBool("Chase", getCurrentEnemyState == EnemyStates.Insect_Chase);
        anmin.SetBool("Stun", getCurrentEnemyState == EnemyStates.Insect_Stunned);
        if(getCurrentEnemyState == EnemyStates.Insect_Death)
        {
            anmin.SetTrigger("Death");
        }
    }
    public override void EnemyGetHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyGetHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            currentHitCount++;  // Tăng số hit
            if (currentHitCount >= hitsBeforeStun)
            {
                ChangeState(EnemyStates.Insect_Stunned);
                currentHitCount = 0;  // Reset lại số hit sau khi choáng
            }
        }
        else
        {
            ChangeState(EnemyStates.Insect_Death);
        }
    }
    public override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }
    private void FlipInsect()
    {
        sr.flipX = PlayerMovement.Instance.transform.position.x < transform.position.x;
    }

}

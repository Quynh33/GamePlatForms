using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIdle : StateMachineBehaviour
{
    Rigidbody2D rb;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      rb = animator.GetComponentInParent<Rigidbody2D>();
    }

   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero;
        RunToPlayer(animator);
        if(Boss.Instance.attackCountdown <= 0)
        {
            Boss.Instance.AttackHandler();
            Boss.Instance.attackCountdown = Random.Range(Boss.Instance.attackTimer - 1, Boss.Instance.attackTimer - 1);
        }
        if (!Boss.Instance.Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }
    }

  void RunToPlayer(Animator animator)
    {
        if(Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) >= Boss.Instance.attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        { return;
        }    
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
    
   }

  
}

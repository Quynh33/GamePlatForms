using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBounce2 : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 _forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * Boss.Instance.rotationDirectionTarget), Mathf.Sin(Mathf.Deg2Rad * Boss.Instance.rotationDirectionTarget));

        rb.AddForce(_forceDirection * 3, ForceMode2D.Impulse);
        Boss.Instance.divingCollider.SetActive(true);
        if (Boss.Instance.Grounded())
        {
            Boss.Instance.divingCollider.SetActive(false);
            if (!callOnce)
            {
                Boss.Instance.ResetAllAttack();
                callOnce = true;

                animator.SetTrigger("Grounded");
            }
        }
    } 

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bounce2");
        animator.ResetTrigger("Grounded");
        callOnce = false;
    }

   
}

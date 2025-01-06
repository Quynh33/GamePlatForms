using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    Rigidbody2D rb;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D not found in parent of Animator!");
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Boss.Instance == null) return;

        TargetPlayerPosition(animator);

        Boss.Instance.attackCountdown -= Time.deltaTime;
        if (Boss.Instance.attackCountdown <= 0)
        {
            Boss.Instance.AttackHandler();
            Boss.Instance.attackCountdown = Random.Range(Boss.Instance.attackTimer - 1, Boss.Instance.attackTimer - 1);
        }
    }

    void TargetPlayerPosition(Animator animator)
    {
        if (Boss.Instance == null) return;

        if (Boss.Instance.Grounded())
        {
            Boss.Instance.Flip();
            Vector2 targetPosition = new Vector2(PlayerMovement.Instance.transform.position.x, rb.position.y);
            Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, Boss.Instance.runSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25); // Simulate falling
        }

        if (Vector2.Distance(PlayerMovement.Instance.transform.position, rb.position) <= Boss.Instance.attackRange)
        {
            animator.SetBool("Run", false);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Boss.Instance == null) return;

        animator.SetBool("Run", false);
    }
}
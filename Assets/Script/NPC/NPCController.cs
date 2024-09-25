using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : Interactable
{
    // Start is called before the first frame update
    public Transform player;
    public float wakeUpRadius = 1f;
    private Animator anim;
    private Rigidbody2D myRigidbody;
    private enum NPCState { sleep, awake};
    private NPCState currentState = NPCState.sleep;
    void Start()
    {
        anim = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }
    public  void CheckDistance()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if(distanceToPlayer <= wakeUpRadius)
        {
            if(currentState == NPCState.sleep)
            {
                anim.SetBool("Wakeup", true);
                currentState = NPCState.awake;
            }
        }
        else
        {
            if (currentState == NPCState.awake)
            {
                anim.SetBool("Wakeup", false);
                currentState = NPCState.sleep;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            context.Raise();
            playerInRange = false;

        }
    }
}

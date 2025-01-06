using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingPillar : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has collided with the pillar");
            other.GetComponent<PlayerMovement>().TakeDamage(1);
        }
    }
}

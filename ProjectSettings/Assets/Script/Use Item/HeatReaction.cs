using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatReaction : MonoBehaviour
{
    public int healthRestoreAmount = 2;  // Amount of health the potion restores
    private PlayerMovement player;       // Reference to the player's movement script

    // Called when the player uses the potion
    public void Use()
    {
        // Ensure that the player reference is set
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        // Restore health to the player
        if (player != null)
        {
            player.Health += healthRestoreAmount;

            // Destroy the potion object after use
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ManaPotion : MonoBehaviour
{
    [SerializeField] float manaGainAmount = 0.5f;  // Amount of mana to restore (half of the total mana bar)          // Reference to the UI image representing the mana bar
    private PlayerMovement player;                // Reference to the player's movement script

    // Called when the player uses the potion
    public void Use()
    {
        // Ensure the player reference is set
        if (player == null)
        {
            player = FindObjectOfType<PlayerMovement>();
        }

        // Increase the player's mana by half of the mana bar
        if (player != null)
        {
            float maxMana = 1f;  // Assuming the maximum mana is 1 (100% of the mana bar)
            float currentMana = player.mana;

            // Add half of the mana bar to the current mana
            player.mana += manaGainAmount * maxMana;
            player.mana = Mathf.Clamp(player.mana, 0, maxMana); // Ensure mana doesn't exceed the max

            // Optionally, update the UI
            player.manaStorage.fillAmount = player.mana;

            // Destroy the potion object after use
        }
    }
}
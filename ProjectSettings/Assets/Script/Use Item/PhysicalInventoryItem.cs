using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalInventoryItem : Powerup
{
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private InventoryItems thisItem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AddItemToInventory();
            Debug.Log("Item được thêm vào inventory và sẽ bị phá hủy.");
            Destroy(this.gameObject);
        }
    }

    void AddItemToInventory()
    {
        if (playerInventory == null || thisItem == null)
        {
            Debug.LogError("playerInventory hoặc thisItem chưa được gán!");
            return;
        }

        if (playerInventory.myInventory.Contains(thisItem))
        {
            thisItem.numberHeld += 1;
            Debug.Log($"Đã tăng số lượng item: {thisItem.name} lên {thisItem.numberHeld}");
        }
        else
        {
            playerInventory.myInventory.Add(thisItem);
            Debug.Log($"Đã thêm item: {thisItem.name} vào inventory");
        }
    }
}
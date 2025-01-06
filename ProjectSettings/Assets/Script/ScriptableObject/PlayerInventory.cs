using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Player Inventory")]
public class PlayerInventory : ScriptableObject
{
    public List<InventoryItems> myInventory = new List<InventoryItems>();

    public void AddItem(InventoryItems item, int amount)
    {
        // Kiểm tra xem item đã tồn tại chưa
        InventoryItems existingItem = myInventory.Find(i => i.itemName == item.itemName);

        if (existingItem != null)
        {
            // Nếu tồn tại, tăng số lượng
            existingItem.numberHeld += amount;
        }
        else
        {
            // Nếu chưa, thêm item mới
            InventoryItems newItem = Instantiate(item);
            newItem.numberHeld = amount;
            myInventory.Add(newItem);
        }
    }
}
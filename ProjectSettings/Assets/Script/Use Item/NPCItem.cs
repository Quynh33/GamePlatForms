using UnityEngine;

public class NPCItem :NPCItemController
{
    [SerializeField] private Inventory thisItem;

    public void ContinueDialogue()
    {
        DialogueManager.GetInstance().ContinueStory();
    }

    public void GiveKey()
    {
        if (playerInventory && itemToGive)
        {
            if (playerInventory.myInventory.Contains(itemToGive))
            {
                itemToGive.numberHeld += 1;
            }
            else
            {
                playerInventory.myInventory.Add(itemToGive);
                itemToGive.numberHeld = 1; // Thiết lập số lượng ban đầu nếu chưa có trong inventory
            }

            // Hiển thị vật phẩm
            ShowItemDisplay(itemToGive);
            DialogueManager.GetInstance().ContinueStory();
        }
        else
        {
            Debug.LogWarning("Player inventory or item to give is null.");
        }
}


        public void ShowItemDisplay(InventoryItems item)
        {
            // Tạo object hiển thị từ prefab
            GameObject itemDisplay = Instantiate(itemDisplayPrefab, displaySpawnPoint.position, Quaternion.identity);

            // Thiết lập hình ảnh và tên vật phẩm
            ItemDisplay displayScript = itemDisplay.GetComponent<ItemDisplay>();
            if (displayScript != null)
            {
                displayScript.SetupItem(item.itemImage, item.itemName);
            }

            // Tùy chọn: Tự động xóa object sau vài giây
            Destroy(itemDisplay, 3f);
        }
    }
 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCItemController : MonoBehaviour
{
    public InventoryItems itemToGive;  // Vật phẩm cấp cho người chơi
    public GameObject itemDisplayPrefab; // Prefab để hiển thị vật phẩm
    public Transform displaySpawnPoint; // Vị trí hiển thị prefab
    public PlayerInventory playerInventory; // Tham chiếu đến PlayerInventory

}

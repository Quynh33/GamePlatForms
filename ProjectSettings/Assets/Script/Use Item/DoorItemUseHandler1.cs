using UnityEngine;

public class DoorItemUseHandler : MonoBehaviour
{
    public string targetDoorKey; // Key của cánh cửa cần mở

    // Phương thức này sẽ được gọi khi sử dụng item
    public void OnItemUsed(InventoryItems item)
    {
        // Tìm đối tượng cánh cửa trong Scene
        GameObject targetDoor = SceneObjectRegistry.Instance.GetObject(targetDoorKey);
        if (targetDoor != null)
        {
            // Gọi phương thức OpenDoor() để mở cửa
            DoorObject doorObject = targetDoor.GetComponent<DoorObject>();
            if (doorObject != null)
            {
                doorObject.OpenDoor();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy component DoorObject trên đối tượng.");
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy đối tượng có key: {targetDoorKey}");
        }
    }
}
using UnityEngine;

public class WaterItemUseHandler : MonoBehaviour
{
    public string targetObjectKey; // Key của đối tượng vùng nước cần thay đổi
    public Color newColor; // Màu mới cho vùng nước

    // Phương thức này sẽ được gọi khi vật phẩm được sử dụng
    public void OnItemUsed(InventoryItems item)
    {
        // Tìm đối tượng vùng nước trong Scene
        GameObject targetObject = SceneObjectRegistry.Instance.GetObject(targetObjectKey);
        if (targetObject != null)
        {
            // Thay đổi màu sắc vùng nước thông qua WaterObject
            WaterObject waterObject = targetObject.GetComponent<WaterObject>();
            if (waterObject != null)
            {
                waterObject.ChangeWaterColor(newColor); // Chỉ thay đổi màu sắc
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy đối tượng có key: {targetObjectKey}");
        }
    }
}
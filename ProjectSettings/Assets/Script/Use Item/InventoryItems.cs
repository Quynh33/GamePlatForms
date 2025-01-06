using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Items")]
public class InventoryItems : ScriptableObject
{
    public string itemName; // Tên vật phẩm
    public string itemDescription; // Mô tả vật phẩm
    public Sprite itemImage; // Hình ảnh vật phẩm
    public int numberHeld; // Số lượng vật phẩm đang giữ
    public bool usable; // Có thể sử dụng hay không
    public bool unique; // Vật phẩm có duy nhất hay không
    public UnityEvent thisEvent; // Sự kiện khi vật phẩm được sử dụng
    public string targetObjectKey; // Key của đối tượng trong scene

    // Phương thức sử dụng vật phẩm
    public void Use()
    {
        Debug.Log($"Using {itemName}");

        // Gọi UnityEvent nếu có gán
        thisEvent.Invoke();

        // Tìm đối tượng trong registry và tương tác với nó
        GameObject targetObject = SceneObjectRegistry.Instance.GetObject(targetObjectKey);
        if (targetObject != null)
        {
            // Ví dụ: Gọi một phương thức trên đối tượng
            targetObject.SendMessage("OnItemUsed", this, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy đối tượng có key: {targetObjectKey}");
        }
    }
    public void DecreaseAmoumt(int amountToDecrease)
    {
        numberHeld -= amountToDecrease;
        if ( numberHeld < 0 )
        {
            numberHeld = 0;
        }
    }

}
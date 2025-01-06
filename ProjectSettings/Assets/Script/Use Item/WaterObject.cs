using UnityEngine;

public class WaterObject : MonoBehaviour
{
    public string waterObjectKey; // Key của đối tượng vùng nước trong Scene
    private Renderer renderer;

    public GameObject objectToDestroy; // GameObject cần phá hủy khi màu nước thay đổi
    public CircleCollider2D[] collidersToDestroy; // Các PolygonCollider2D sẽ bị phá hủy
    public Animator[] eggAnimators; // Danh sách Animator của các quả trứng

    private void Start()
    {
        // Lấy Renderer để thay đổi màu sắc của vật liệu
        renderer = GetComponent<Renderer>();

        // Đăng ký đối tượng vào SceneObjectRegistry nếu có key
        if (!string.IsNullOrEmpty(waterObjectKey))
        {
            SceneObjectRegistry.Instance.RegisterObject(waterObjectKey, this.gameObject);
        }

        // Đảm bảo các Collider và Animator được cấu hình đúng
        if (collidersToDestroy == null || collidersToDestroy.Length == 0)
        {
            Debug.LogWarning("Không có PolygonCollider2D nào được chỉ định để phá hủy.");
        }

        if (eggAnimators == null || eggAnimators.Length == 0)
        {
            Debug.LogWarning("Không có Animator nào được gán cho quả trứng!");
        }
    }

    public void ChangeWaterColor(Color newColor)
    {
        // Thay đổi màu sắc của vật liệu
        if (renderer != null)
        {
            renderer.material.color = newColor;
        }

        // Phá hủy đối tượng nếu có
        if (objectToDestroy != null)
        {
            Destroy(objectToDestroy);
        }

        // Phá hủy các PolygonCollider2D nếu có
        if (collidersToDestroy != null && collidersToDestroy.Length > 0)
        {
            foreach (var collider in collidersToDestroy)
            {
                if (collider != null)
                {
                    Destroy(collider); // Xóa Collider để đối tượng không còn bị giới hạn
                }
            }
        }

        // Chuyển trạng thái của tất cả quả trứng
        if (eggAnimators != null && eggAnimators.Length > 0)
        {
            foreach (var animator in eggAnimators)
            {
                if (animator != null)
                {
                    animator.SetBool("Break", true); // Kích hoạt animation với trạng thái "break"
                }
            }
        }
    }
}
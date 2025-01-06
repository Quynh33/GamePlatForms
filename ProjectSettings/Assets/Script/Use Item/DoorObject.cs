using UnityEngine;

public class DoorObject : MonoBehaviour
{
    public string doorObjectKey; // Key của đối tượng cánh cửa
    private Animator animator;

    private void Start()
    {
        // Lấy Animator Component
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"Animator không được tìm thấy trên đối tượng {gameObject.name}. Hãy gắn Animator vào đối tượng.");
        }

        // Đăng ký đối tượng vào SceneObjectRegistry nếu có key
        if (!string.IsNullOrEmpty(doorObjectKey))
        {
            SceneObjectRegistry.Instance.RegisterObject(doorObjectKey, this.gameObject);
        }
    }

    // Mở cửa bằng cách chuyển sang Animation Open
    public void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetBool("isOpen", true); // Kích hoạt Animation "Open"
        }
        else
        {
            Debug.LogWarning($"Animator không được gắn trên đối tượng {gameObject.name}. Không thể mở cửa.");
        }
    }
}